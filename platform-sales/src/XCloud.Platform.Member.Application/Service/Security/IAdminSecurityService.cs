using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using XCloud.Core.Application;
using XCloud.Core.Dto;
using XCloud.Platform.Core.Application;
using XCloud.Platform.Core.Domain.Security;
using XCloud.Platform.Data.Database;
using XCloud.Platform.Member.Application.Service.Admin;

namespace XCloud.Platform.Member.Application.Service.Security;

public interface IAdminSecurityService : IXCloudApplicationService
{
    Task<GrantedPermissionResponse> GetGrantedPermissionsAsync(GetGrantedPermissionInput dto,
        CachePolicy cachePolicyOption);
}

public class AdminSecurityService : PlatformApplicationService, IAdminSecurityService
{
    private readonly IMemberRepository<SysRole> _roleRepository;
    private readonly IAdminService _adminService;
    private readonly IAbpPermissionService _abpPermissionService;

    public AdminSecurityService(IMemberRepository<SysRole> roleRepository,
        IAbpPermissionService abpPermissionService,
        IAdminService adminService)
    {
        this._roleRepository = roleRepository;
        _abpPermissionService = abpPermissionService;
        _adminService = adminService;
    }

    private async Task<GrantedPermissionResponse> GetGrantedPermissionsAsync(GetGrantedPermissionInput dto)
    {
        var db = await _roleRepository.GetDbContextAsync();

        var query = from rolePermission in db.Set<SysRolePermission>().AsNoTracking()
            join roleAdmin in db.Set<SysAdminRole>().AsNoTracking()
                on rolePermission.RoleId equals roleAdmin.RoleId
            select new { rolePermission, roleAdmin };

        query = query.Where(x => x.roleAdmin.AdminId == dto.AdminId);

        if (!string.IsNullOrWhiteSpace(dto.AppKey))
        {
            throw new NotImplementedException();
        }

        //permissions
        var permissionKeys = await query
            .Select(x => x.rolePermission.PermissionKey)
            .Distinct()
            .ToArrayAsync();

        var data = new GrantedPermissionResponse
        {
            RecalledPermissions = default,
            Permissions = permissionKeys,
        };

        //admin dto
        var admin = await this._adminService.GetAdminByIdAsync(dto.AdminId);
        if (admin != null)
        {
            data.SetData(admin);
        }

        return data;
    }

    public async Task<GrantedPermissionResponse> GetGrantedPermissionsAsync(GetGrantedPermissionInput dto,
        CachePolicy cachePolicyOption)
    {
        var key = $"{dto.AdminId}.permission.keys.response";

        var option = new CacheOption<GrantedPermissionResponse>(key);

        var data = await this.CacheProvider.ExecuteWithPolicyAsync(
            () => GetGrantedPermissionsAsync(dto),
            option, cachePolicyOption);

        data ??= new GrantedPermissionResponse();

        return data;
    }
}