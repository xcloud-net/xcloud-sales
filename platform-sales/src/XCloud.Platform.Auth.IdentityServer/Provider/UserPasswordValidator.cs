using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using IdentityServer4.Models;
using IdentityServer4.Validation;
using Volo.Abp;
using XCloud.Core.Application.WorkContext;
using XCloud.Core.Dto;
using XCloud.Core.Extension;
using XCloud.Platform.Application.Member.Service.User;
using XCloud.Platform.Auth.Authentication;

namespace XCloud.Platform.Auth.IdentityServer.Provider;

public class UserPasswordValidator : IResourceOwnerPasswordValidator
{
    private readonly IWorkContext _workContext;
    private readonly IUserAccountService _userLogin;

    public UserPasswordValidator(IWorkContext<UserPasswordValidator> workContext, IUserAccountService userLogin)
    {
        this._workContext = workContext;
        _userLogin = userLogin;
    }

    public async Task ValidateAsync(ResourceOwnerPasswordValidationContext context)
    {
        try
        {
            var dto = new PasswordLoginDto()
            {
                IdentityName = context.UserName,
                Password = context.Password
            };
            var res = await _userLogin.PasswordLoginAsync(dto);
            res.ThrowIfErrorOccured();

            var model = res.Data;

            var identity = new ClaimsIdentity().SetSubjectId(model.Id)
                .SetIdentityGrantsAll()
                .SetCreationTime(DateTime.UtcNow);

            //这个返回还没用到
            var response = new Dictionary<string, object>()
            {
                ["new_user"] = true,
                ["password"] = "pwd",
                ["time_utc"] = DateTime.UtcNow
            };

            context.Result = new GrantValidationResult(
                subject: model.Id,
                claims: identity.Claims,
                authenticationMethod: "custom",
                customResponse: response);
        }
        catch (BusinessException e)
        {
            var msg = e.Message ?? nameof(UserPasswordValidator);
            context.Result = new GrantValidationResult(TokenRequestErrors.InvalidRequest, msg);
        }
        catch (Exception e)
        {
            _workContext.Logger.AddErrorLog(e.Message, e);
            context.Result = new GrantValidationResult(TokenRequestErrors.InvalidGrant, "服务器发生错误");
        }
    }
}