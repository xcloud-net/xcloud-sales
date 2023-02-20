using System.Collections.Generic;
using System.Threading.Tasks;
using FluentAssertions;
using Volo.Abp.Application.Services;
using XCloud.Core.Application;
using XCloud.Core.Dto;
using XCloud.Database.EntityFrameworkCore.Extensions;
using XCloud.Platform.Core.Application;
using XCloud.Platform.Core.Domain.Menu;
using XCloud.Platform.Data.Database;

namespace XCloud.Platform.Common.Application.Service.Menu;

public interface IMenuService : IApplicationService
{
    Task<ApiResponse<SysMenu>> AddMenuAsync(SysMenu model);

    Task<SysMenu> GetMenuByIdAsync(string uid);

    Task<bool> DeleteMenuWhenNoChildrenAsync(string uid);

    Task<List<SysMenu>> QueryMenuByGroupAsync(string groupKey, string parent = null);

    Task UpdateMenuAsync(SysMenu model);
}

public class MenuService : PlatformApplicationService, IMenuService
{
    private readonly IPlatformRepository<SysMenu> _menuRepo;

    public MenuService(IPlatformRepository<SysMenu> _menuRepo)
    {
        this._menuRepo = _menuRepo;
    }

    public virtual async Task<ApiResponse<SysMenu>> AddMenuAsync(SysMenu model)
    {
        model.Should().NotBeNull();
        model.TreeGroupKey.Should().NotBeNullOrEmpty();

        model.Id = this.GuidGenerator.CreateGuidString();
        model.CreationTime = this.Clock.Now;

        return await _menuRepo.AddTreeNode(model);
    }

    public virtual async Task<List<SysMenu>> QueryMenuByGroupAsync(string group_key, string parent = null)
    {
        group_key.Should().NotBeNullOrEmpty("query menu list group key");

        var query = await _menuRepo.GetQueryableAsync();
        query = query.Where(x => x.TreeGroupKey == group_key);
        query = query.WhereIf(!string.IsNullOrWhiteSpace(parent), x => x.ParentId == parent);

        var res = query.OrderBy(x => x.Sort).Take(5000).ToList();
        return res;
    }

    public virtual async Task UpdateMenuAsync(SysMenu model)
    {
        model.Should().NotBeNull("update menu model");
        model.Id.Should().NotBeNullOrEmpty("update menu uid");

        var menu = await _menuRepo.QueryOneAsync(x => x.Id == model.Id);
        menu.Should().NotBeNull("菜单不存在");

        menu.Name = model.Name;
        menu.PermissionJson = model.PermissionJson;

        menu.SetEntityFields(new
        {
            model.Name,
            model.Description,
            model.Url,
            model.IconCls,
            model.IconUrl,
            model.PermissionJson,
            model.Sort
        });

        menu.LastModificationTime = this.Clock.Now;

        await _menuRepo.UpdateNowAsync(menu);
    }

    public virtual async Task<bool> DeleteMenuWhenNoChildrenAsync(string uid)
    {
        uid.Should().NotBeNullOrEmpty("delete menu uid");

        var res = await _menuRepo.DeleteSingleNodeWhenNoChildren(uid);
        return res;
    }

    public virtual async Task<SysMenu> GetMenuByIdAsync(string uid)
    {
        uid.Should().NotBeNullOrEmpty("get menu by uid,uid");

        var res = await _menuRepo.QueryOneAsync(x => x.Id == uid);

        return res;
    }
}