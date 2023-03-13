using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using XCloud.Platform.Core.Application;
using XCloud.Platform.Core.Domain.Security;
using XCloud.Platform.Data.Database;

namespace XCloud.Platform.Application.Member.Service.Security;

public interface IPermissionService : IPlatformCrudAppService<SysPermission, SysPermissionDto>
{
    //
}

public class PermissionService : PlatformCrudAppService<SysPermission, SysPermissionDto>, IPermissionService
{
    public PermissionService(IMemberRepository<SysPermission> repository) : base(repository)
    {
        //
    }

    private async Task<bool> CheckNameIsExistAsync(DbContext db,
        string permissionKey, string exceptIdOrEmpty = null)
    {
        if (string.IsNullOrWhiteSpace(permissionKey))
            throw new ArgumentNullException(nameof(permissionKey));

        var query = db.Set<SysPermission>().AsNoTracking();
        if (!string.IsNullOrWhiteSpace(exceptIdOrEmpty))
            query = query.Where(x => x.Id != exceptIdOrEmpty);

        var exist = await query.AnyAsync(x => x.PermissionKey == permissionKey);

        return exist;
    }

    protected override async Task CheckBeforeInsertAsync(SysPermissionDto dto)
    {
        var db = await this.Repository.GetDbContextAsync();

        if (await this.CheckNameIsExistAsync(db, dto.PermissionKey))
            throw new UserFriendlyException("permission key exist");
    }

    protected override async Task CheckBeforeUpdateAsync(SysPermissionDto dto)
    {
        var db = await this.Repository.GetDbContextAsync();

        if (await this.CheckNameIsExistAsync(db, dto.PermissionKey, exceptIdOrEmpty: dto.Id))
            throw new UserFriendlyException("permission key exist");
    }

    protected override async Task ModifyFieldsForUpdateAsync(SysPermission entity, SysPermissionDto dto)
    {
        await Task.CompletedTask;
        entity.Description = dto.Description;
    }
}