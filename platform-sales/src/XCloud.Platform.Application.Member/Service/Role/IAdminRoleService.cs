using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using XCloud.Application.Service;
using XCloud.Core.Application.Entity;
using XCloud.Core.IdGenerator;
using XCloud.Database.EntityFrameworkCore.Extensions;
using XCloud.Platform.Application.Member.Service.Admin;
using XCloud.Platform.Application.Member.Service.Security;
using XCloud.Platform.Core.Application;
using XCloud.Platform.Core.Domain.Security;
using XCloud.Platform.Data.Database;

namespace XCloud.Platform.Application.Member.Service.Role;

public interface IAdminRoleService : IXCloudApplicationService
{
    Task<SysRole[]> QueryByAdminIdAsync(string adminId, CachePolicy cachePolicy);

    Task SetAdminRolesAsync(string adminId, string[] roleIds);

    Task<SysAdminDto[]> AttachRolesAsync(SysAdminDto[] data);
}

public class AdminRoleService : PlatformApplicationService, IAdminRoleService
{
    private readonly IMemberRepository<SysRole> _roleRepository;

    public AdminRoleService(IMemberRepository<SysRole> roleRepository)
    {
        _roleRepository = roleRepository;
    }

    public async Task<SysAdminDto[]> AttachRolesAsync(SysAdminDto[] data)
    {
        if (!data.Any())
            return data;

        var db = await this._roleRepository.GetDbContextAsync();

        var query = from adminRole in db.Set<SysAdminRole>().AsNoTracking()
            join role in db.Set<SysRole>().AsNoTracking()
                on adminRole.RoleId equals role.Id
            select new { adminRole, role };

        var ids = data.Ids().ToArray();

        var datalist = await query
            .Where(x => ids.Contains(x.adminRole.AdminId)).ToArrayAsync();

        foreach (var m in data)
        {
            var roles = datalist.Where(x => x.adminRole.AdminId == m.Id).Select(x => x.role).ToArray();
            m.Roles = roles.Select(x => this.ObjectMapper.Map<SysRole, SysRoleDto>(x)).ToArray();
        }

        return data;
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

    public async Task<SysRole[]> QueryByAdminIdAsync(string adminId, CachePolicy cachePolicy)
    {
        var key = $"roles.entity.by.admin.id:{adminId}";
        var option = new CacheOption<SysRole[]>(key, TimeSpan.FromMinutes(5));

        var response =
            await this.CacheProvider.ExecuteWithPolicyAsync(
                () => this.QueryByAdminIdAsync(adminId),
                option,
                cachePolicy);

        response ??= Array.Empty<SysRole>();
        
        return response;
    }

    public async Task<SysRole[]> QueryByAdminIdAsync(string adminId)
    {
        if (string.IsNullOrWhiteSpace(adminId))
            throw new ArgumentNullException(nameof(adminId));

        var db = await this._roleRepository.GetDbContextAsync();

        var assignQuery = db.Set<SysAdminRole>().AsNoTracking()
            .Where(x => x.AdminId == adminId);

        var roleQuery = db.Set<SysRole>().AsNoTracking();

        var query = from assign in assignQuery
            join role in roleQuery
                on assign.RoleId equals role.Id
            select new { assign, role };

        var data = await query.OrderBy(x => x.role.CreationTime).ToArrayAsync();

        var response = data.Select(x => x.role).ToArray();

        return response;
    }

    public async Task SetAdminRolesAsync(string adminId, string[] roleIds)
    {
        if (string.IsNullOrWhiteSpace(adminId))
            throw new ArgumentNullException(nameof(adminId));

        if (roleIds == null)
            throw new ArgumentNullException(nameof(roleIds));

        var assigns = roleIds.Select(x => new SysAdminRole()
        {
            RoleId = x,
            AdminId = adminId,
        }).ToArray();

        var db = await this._roleRepository.GetDbContextAsync();

        var set = db.Set<SysAdminRole>();

        var originData = await set.Where(x => x.AdminId == adminId).ToArrayAsync();

        static string FingerPrint(SysAdminRole entity) => $"{entity.RoleId}";

        var toDelete = originData.NotInBy(assigns, FingerPrint).ToArray();
        var toInsert = assigns.NotInBy(originData, FingerPrint).ToArray();

        if (toDelete.Any())
            set.RemoveRange(toDelete);

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
}