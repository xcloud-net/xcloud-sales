using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using XCloud.Core.Application;
using XCloud.Platform.Core.Application;
using XCloud.Platform.Core.Domain.AdminPermission;
using XCloud.Platform.Data.Database;

namespace XCloud.Platform.Member.Application.Service.AdminPermission;

public interface IRoleService : IPlatformPagingCrudService<SysRole, SysRoleDto, QueryRolePagingInput>
{
    //
}

public class RoleService : PlatformPagingCrudService<SysRole, SysRoleDto, QueryRolePagingInput>, IRoleService
{
    public RoleService(IMemberRepository<SysRole> repository) : base(repository)
    {
        //
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