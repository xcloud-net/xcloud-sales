using System.Threading.Tasks;
using Volo.Abp.DependencyInjection;
using XCloud.Core.Dto;
using XCloud.Core.IdGenerator;
using XCloud.Platform.Core.Application;
using XCloud.Platform.Core.Domain.Admin;
using XCloud.Platform.Core.Domain.User;
using XCloud.Platform.Member.Application.Service.Admin;
using XCloud.Platform.Member.Application.Service.Security;
using XCloud.Platform.Member.Application.Service.User;

namespace XCloud.Platform.Test.Service;

[ExposeServices(typeof(TestUserAccountService))]
public class TestUserAccountService : PlatformApplicationService
{
    private readonly IUserAccountService _userAccountService;
    private readonly IUserProfileService _userProfileService;
    private readonly IAdminAccountService _adminAccountService;
    private readonly IRoleService _roleService;
    private readonly IAdminRoleService _adminRoleService;
    private readonly IAdminSecurityService _adminSecurityService;

    public TestUserAccountService(IUserAccountService userAccountService, IAdminAccountService adminAccountService,
        IRoleService roleService, IAdminRoleService adminRoleService, IAdminSecurityService adminSecurityService,
        IUserProfileService userProfileService)
    {
        _userAccountService = userAccountService;
        _adminAccountService = adminAccountService;
        _roleService = roleService;
        _adminRoleService = adminRoleService;
        _adminSecurityService = adminSecurityService;
        _userProfileService = userProfileService;
    }

    public async Task TestUserAccountAsync()
    {
        var user = await this.CreateUserAccountAsync();
        var admin = await this.CreateAdminAccountAsync(user);
    }

    private async Task<SysAdmin> CreateAdminAccountAsync(SysUser user)
    {
        var response = await this._adminAccountService.CreateAdminAccountAsync(new CreateAdminDto(user.Id));

        response.ThrowIfErrorOccured();

        await this._adminAccountService.UpdateAdminStatusAsync(new UpdateAdminStatusDto()
        {
            Id = response.Data.Id,
            IsActive = true,
            IsSuperAdmin = false
        });

        return response.Data;
    }

    private async Task<SysUser> CreateUserAccountAsync()
    {
        var userName = this.GuidGenerator.CreateGuidString();
        var response = await this._userAccountService.CreateUserAccountAsync(new IdentityNameDto(userName));

        response.ThrowIfErrorOccured();

        await this._userAccountService.SetPasswordAsync(response.Data.Id, "123");

        await this._userAccountService.UpdateUserStatusAsync(new UpdateUserStatusDto()
        {
            Id = response.Data.Id,
            IsActive = true,
            IsDeleted = false,
        });

        await this._userProfileService.UpdateNickNameAsync(response.Data.Id, $"test-user-{this.Clock.Now.Ticks}");

        return response.Data;
    }
}