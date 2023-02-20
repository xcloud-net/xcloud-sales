using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using XCloud.Core.Application;
using XCloud.Core.Dto;
using XCloud.Core.IdGenerator;
using XCloud.Database.EntityFrameworkCore.Extensions;
using XCloud.Platform.Core.Application;
using XCloud.Platform.Core.Domain.AdminPermission;
using XCloud.Platform.Data.Database;
using XCloud.Platform.Member.Application.Service.Admin;

namespace XCloud.Platform.Member.Application.Service.AdminPermission;

public interface IAdminPermissionService : IXCloudApplicationService
{
    Task SetRolePermissionsAsync(string roleId, string[] permissionKeys);

    Task<GrantedPermissionResponse> GetGrantedPermissionsAsync(GetGrantedPermissionInput dto,
        CachePolicy cachePolicyOption);
}

public class AdminPermissionService : PlatformApplicationService, IAdminPermissionService
{
    private readonly IMemberRepository<SysRole> _roleRepository;
    private readonly IAdminService _adminService;
    private readonly IAbpPermissionService _abpPermissionService;

    public AdminPermissionService(IMemberRepository<SysRole> roleRepository,
        IAbpPermissionService abpPermissionService,
        IAdminService adminService)
    {
        this._roleRepository = roleRepository;
        _abpPermissionService = abpPermissionService;
        _adminService = adminService;
    }

    public async Task SetRolePermissionsAsync(string roleId, string[] permissionKeys)
    {
        if (string.IsNullOrWhiteSpace(roleId))
            throw new ArgumentNullException(nameof(roleId));

        if (permissionKeys == null)
            throw new ArgumentNullException(nameof(permissionKeys));

        var assigns = permissionKeys.Select(x => new SysRolePermission()
        {
            PermissionKey = x,
            RoleId = roleId,
        }).ToArray();

        var db = await this._roleRepository.GetDbContextAsync();

        var set = db.Set<SysRolePermission>();

        var originData = await set.Where(x => x.RoleId == roleId).ToArrayAsync();

        static string FingerPrint(SysRolePermission entity) =>
            $"{entity.PermissionKey}";

        var toDelete = originData.NotInBy(assigns, FingerPrint).ToArray();
        var toInsert = assigns.NotInBy(originData, FingerPrint).ToArray();

        if (toDelete.Any())
        {
            set.RemoveRange(toDelete);
        }

        if (toInsert.Any())
        {
            var now = this.Clock.Now;
            foreach (var m in toInsert)
            {
                m.Id = this.GuidGenerator.CreateGuidString();
                m.CreationTime = now;
                set.Add(m);
            }
        }

        await db.TrySaveChangesAsync();
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