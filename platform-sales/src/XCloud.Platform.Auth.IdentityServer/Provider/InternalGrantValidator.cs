using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using IdentityServer4.Models;
using IdentityServer4.Validation;
using Volo.Abp;
using XCloud.Core.Application.WorkContext;
using XCloud.Core.Extension;
using XCloud.Platform.Application.Common.Service.Token;
using XCloud.Platform.Auth.Authentication;
using XCloud.Platform.Shared.Constants;

namespace XCloud.Platform.Auth.IdentityServer.Provider;

public class InternalGrantValidator : IExtensionGrantValidator
{
    private readonly TempCodeService _internalGrantService;
    private readonly IWorkContext _workContext;
    public InternalGrantValidator(TempCodeService internalGrantService, IWorkContext<InternalGrantValidator> workContext)
    {
        this._internalGrantService = internalGrantService;
        this._workContext = workContext;
    }

    public string GrantType => IdentityConsts.GrantType.InternalGrantType;

    public async Task ValidateAsync(ExtensionGrantValidationContext context)
    {
        try
        {
            var key = context.Request.Raw["key"];
            var id = context.Request.Raw["id"];

            var code = await this._internalGrantService.GetTempCode(key);

            if (code == null || code.Id != id)
            {
                throw new BusinessException("temp code is not found");
            }

            await this._internalGrantService.RemoveTempCode(key);

            var identity = new ClaimsIdentity()
                .SetSubjectId(code.Id)
                .SetCreationTime(DateTime.UtcNow);

            //这个返回还没用到
            var response = new Dictionary<string, object>()
            {
                ["temp_key"] = key,
                ["id"] = id
            };

            context.Result = new GrantValidationResult(
                subject: code.Id,
                claims: identity.Claims,
                authenticationMethod: "custom",
                customResponse: response);
        }
        catch (BusinessException e)
        {
            var msg = e.Message ?? nameof(InternalGrantValidator);
            context.Result = new GrantValidationResult(TokenRequestErrors.InvalidGrant, msg);
        }
        catch (Exception e)
        {
            this._workContext.Logger.AddErrorLog($"{e.Message}", e);
            context.Result = new GrantValidationResult(TokenRequestErrors.InvalidGrant, "服务器发生错误");
        }
    }
}