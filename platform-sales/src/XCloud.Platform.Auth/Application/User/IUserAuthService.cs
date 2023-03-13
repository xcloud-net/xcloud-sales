using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Volo.Abp;
using XCloud.Application.Service;
using XCloud.Core.Dto;
using XCloud.Platform.Auth.Authentication;
using XCloud.Platform.Core.Application;
using XCloud.Platform.Application.Member.Service.User;

namespace XCloud.Platform.Auth.Application.User;


public interface IUserAuthService : IXCloudApplicationService
{
    Task<UserAuthResponse> GetAuthUserAsync();
}

public class UserAuthService : PlatformApplicationService, IUserAuthService
{
    private readonly IUserAccountService _userAccountService;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IPlatformAuthResultHolder _platformAuthResultHolder;

    public UserAuthService(
        IUserAccountService userAccountService,
        IHttpContextAccessor httpContextAccessor, 
        IPlatformAuthResultHolder platformAuthResultHolder)
    {
        this._userAccountService = userAccountService;
        this._httpContextAccessor = httpContextAccessor;
        _platformAuthResultHolder = platformAuthResultHolder;
    }

    private async Task<ClaimsPrincipal> AuthenticationAsync()
    {
        await Task.CompletedTask;

        if (this._httpContextAccessor.HttpContext == null)
            throw new AbpException(nameof(this._httpContextAccessor));

        return this._httpContextAccessor.HttpContext.User;
    }

    public async Task<UserAuthResponse> GetAuthUserAsync()
    {
        var cachedResponse = this._platformAuthResultHolder.AuthSysUser;
        if (cachedResponse != null)
            return cachedResponse;
            
        var res = new UserAuthResponse();

        try
        {
            var authResult = await this.AuthenticationAsync();
            if (authResult == null || !authResult.IsAuthenticated(out var claims))
            {
                res.SetError("auth failed");
                return res;
            }

            var userId = claims.GetSubjectId();
            var tokenCreationTime = claims.GetCreationTime();
            if (string.IsNullOrWhiteSpace(userId) || tokenCreationTime == null)
            {
                res.SetError("token is not created correctly");
                return res;
            }

            //validate user
            res = await this._userAccountService.GetUserAuthResultAsync(userId);

            if (!res.IsSuccess())
            {
                return res;
            }

            //check token if is expired
            var userData = res.Data;
            
            if (userData.LastPasswordUpdateTime != null &&
                userData.LastPasswordUpdateTime.Value > tokenCreationTime)
            {
                res.SetError("your login is expired");
                return res;
            }
            
            return res;
        }
        finally
        {
            this._platformAuthResultHolder.AuthSysUser = res;
        }
    }
}