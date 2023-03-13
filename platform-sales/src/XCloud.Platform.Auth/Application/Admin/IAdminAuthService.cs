using System.Threading.Tasks;
using XCloud.Application.Service;
using XCloud.Platform.Core.Application;
using XCloud.Platform.Application.Member.Service.Admin;
using XCloud.Platform.Application.Member.Service.User;

namespace XCloud.Platform.Auth.Application.Admin;

public interface IAdminAuthService : IXCloudApplicationService
{
    Task<AdminAuthResponse> GetAuthAdminAsync(SysUserDto currentAuthUser);
}

public class AdminAuthService : PlatformApplicationService, IAdminAuthService
{
    private readonly IAdminAccountService _adminAccountService;
    private readonly IPlatformAuthResultHolder _memberAuthContext;

    public AdminAuthService(IAdminAccountService adminAccountService,
        IPlatformAuthResultHolder memberAuthContext)
    {
        this._memberAuthContext = memberAuthContext;
        this._adminAccountService = adminAccountService;
    }

    public async Task<AdminAuthResponse> GetAuthAdminAsync(SysUserDto currentAuthUser)
    {
        var cachedResponse = this._memberAuthContext.AuthSysAdmin;
        if (cachedResponse != null)
            return cachedResponse;

        var res = new AdminAuthResponse();

        try
        {
            res = await this._adminAccountService.GetAdminAuthResultByUserIdAsync(currentAuthUser.UserId);
            return res;
        }
        finally
        {
            this._memberAuthContext.AuthSysAdmin = res;
        }
    }
}