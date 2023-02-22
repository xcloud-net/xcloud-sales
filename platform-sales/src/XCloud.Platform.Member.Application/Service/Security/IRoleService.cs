using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using XCloud.Core.Application;
using XCloud.Core.IdGenerator;
using XCloud.Database.EntityFrameworkCore.Extensions;
using XCloud.Platform.Core.Application;
using XCloud.Platform.Core.Domain.Security;
using XCloud.Platform.Data.Database;

namespace XCloud.Platform.Member.Application.Service.Security;

public interface IRoleService : IPlatformPagingCrudService<SysRole, SysRoleDto, QueryRolePagingInput>
{
    Task SetRolePermissionsAsync(string roleId, string[] permissionKeys);
    
    Task<SysRoleDto[]> AttachPermissionAsync(SysRoleDto[] data);
}

public class RoleService : PlatformPagingCrudService<SysRole, SysRoleDto, QueryRolePagingInput>, IRoleService
{
    public RoleService(IMemberRepository<SysRole> repository) : base(repository)
    {
        //
    }
    
    public async Task<SysRoleDto[]> AttachPermissionAsync(SysRoleDto[] data)
    {
        if (!data.Any())
            return data;

        var db = await this.Repository.GetDbContextAsync();

        var assignQuery = db.Set<SysRolePermission>().AsNoTracking();

        var query = from assign in assignQuery
            join role in db.Set<SysRole>().AsNoTracking()
                on assign.RoleId equals role.Id
            select new { assign, role };

        var ids = data.Ids().ToArray();

        var rolePermissions = await query.Where(x => ids.Contains(x.role.Id)).ToArrayAsync();

        foreach (var m in data)
        {
            m.PermissionKeys = rolePermissions
                .Where(x => x.role.Id == m.Id)
                .Select(x => x.assign.PermissionKey).ToArray();
        }

        return data;
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

        var db = await this.Repository.GetDbContextAsync();

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

    private async Task<bool> CheckNameIsExistAsync(DbContext db,
        string name, string exceptIdOrEmpty = null)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentNullException(nameof(name));

        var query = db.Set<SysRole>().AsNoTracking();
        if (!string.IsNullOrWhiteSpace(exceptIdOrEmpty))
            query = query.Where(x => x.Id != exceptIdOrEmpty);

        var exist = await query.AnyAsync(x => x.Name == name);

        return exist;
    }

    protected override async Task CheckBeforeInsertAsync(SysRoleDto dto)
    {
        var db = await this.Repository.GetDbContextAsync();

        if (await this.CheckNameIsExistAsync(db, dto.Name))
            throw new UserFriendlyException("role name exist");
    }

    protected override async Task CheckBeforeUpdateAsync(SysRoleDto dto)
    {
        var db = await this.Repository.GetDbContextAsync();

        if (await this.CheckNameIsExistAsync(db, dto.Name, exceptIdOrEmpty: dto.Id))
            throw new UserFriendlyException("role name exist");
    }

    protected override async Task ModifyFieldsForUpdateAsync(SysRole entity, SysRoleDto dto)
    {
        await Task.CompletedTask;
        entity.Name = dto.Name;
        entity.Description = dto.Description;
    }
}