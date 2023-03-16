using System;
using System.Net.Http;
using System.Threading.Tasks;
using IdentityModel.Client;
using Microsoft.AspNetCore.Mvc;
using Volo.Abp;
using XCloud.Core.Application.WorkContext;
using XCloud.Core.Dto;
using XCloud.Core.Extension;
using XCloud.Core.Helper;
using XCloud.Core.IdGenerator;
using XCloud.Platform.Application.Common.Service.Token;
using XCloud.Platform.Application.Member.Service.User;
using XCloud.Platform.Auth.Application.User;
using XCloud.Platform.Auth.Authentication;
using XCloud.Platform.Auth.Configuration;
using XCloud.Platform.Auth.IdentityServer;
using XCloud.Platform.Framework.Controller;
using XCloud.Platform.Shared.Constants;

namespace XCloud.Platform.Api.Controller;

[Route("/api/platform/user/mobile-auth")]
public class UserMobileAuthController : PlatformBaseController, IUserController
{
    private readonly HttpClient _httpClient;
    private readonly IWorkContext _workContext;
    private readonly IUserMobileService _userMobileService;
    private readonly IUserAccountService _userAccountService;
    private readonly TempCodeService _tempCodeService;
    private readonly IValidationTokenService _validationTokenService;

    public UserMobileAuthController(TempCodeService tempCodeService,
        IUserAccountService userAccountService,
        IValidationTokenService validationTokenService,
        IUserMobileService userMobileService,
        IWorkContext<UserAuthController> workContext,
        IHttpClientFactory factory)
    {
        this._userMobileService = userMobileService;
        this._tempCodeService = tempCodeService;
        this._userAccountService = userAccountService;
        this._validationTokenService = validationTokenService;
        this._workContext = workContext;
        this._httpClient = factory.CreateClient("wx_login_");
    }

    private string SmsLoginValidationCodeGroup => "sms-login";

    [HttpPost("send-sms-code")]
    public async Task<ApiResponse<object>> SendSmsCodeAsync([FromBody] SendSmsInput dto)
    {
        dto.Mobile = this._userMobileService.NormalizeMobilePhone(dto.Mobile);

        var user = await this._userMobileService.GetUserByPhoneAsync(dto.Mobile);

        if (user == null)
        {
            if (await this._userMobileService.IsPhoneExistAsync(dto.Mobile))
                throw new UserFriendlyException("the account associated to this phone number is not available");

            var createUserResponse = await this._userAccountService.CreateUserAccountAsync(new IdentityNameDto()
                { IdentityName = this.GuidGenerator.CreateGuidString() });
            createUserResponse.ThrowIfErrorOccured();

            var setMobileResponse =
                await this._userMobileService.SetUserMobilePhoneAsync(createUserResponse.Data.Id, dto.Mobile);
            setMobileResponse.ThrowIfErrorOccured();

            await this._userMobileService.ConfirmMobileAsync(setMobileResponse.Data.Id);

            user = createUserResponse.Data;
        }

        var rand = new Random((int)this.Clock.Now.Ticks);
        var charPool = "123456789".ToArray();

        var code = string.Join(string.Empty, Com.Range(4).Select(x => rand.Choice(charPool)).ToArray());

        await this._userMobileService.SendSmsAsync(new UserPhoneBindSmsMessage() { Phone = dto.Mobile, Code = code });
        await this._validationTokenService.AddValidationCodeAsync(this.SmsLoginValidationCodeGroup, user.Id, code);

        return new ApiResponse<object>();
    }

    [HttpPost("sms-login")]
    public async Task<ApiResponse<AuthTokenDto>> SmsLoginAsync([FromBody] SmsLoginInput dto)
    {
        dto.Mobile = this._userMobileService.NormalizeMobilePhone(dto.Mobile);

        var user = await this._userMobileService.GetUserByPhoneAsync(dto.Mobile);

        if (user == null)
            throw new UserFriendlyException("the account is not available");

        var codeResponse =
            await this._validationTokenService.GetValidationCodeAsync(this.SmsLoginValidationCodeGroup, user.Id);

        if (codeResponse == null || codeResponse.Code != dto.SmsCode ||
            codeResponse.CreateTime < this.Clock.Now.AddMinutes(-5))
            throw new UserFriendlyException("wrong sms code");

        //create temp key
        var tempCode = await this._tempCodeService.CreateTempCode(new IdDto(user.Id));

        var _oAuthConfig = this.Configuration.GetOAuthServerOption();

        //create access token
        var disco = await this._httpClient.GetIdentityServerDiscoveryDocuments(this._workContext.Configuration);
        var tokenResponse = await this._httpClient.RequestTokenAsync(new TokenRequest
        {
            Address = disco.TokenEndpoint,
            GrantType = AuthConstants.GrantType.InternalGrantType,

            ClientId = _oAuthConfig.ClientId,
            ClientSecret = _oAuthConfig.ClientSecret,

            Parameters =
            {
                { "key", tempCode },
                { "id", user.Id }
            }
        });

        var res = tokenResponse.ToAuthTokenDto();
        res.ThrowIfErrorOccured();

        return res;
    }
}