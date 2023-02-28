using System.Collections.Generic;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Volo.Abp;
using XCloud.Core.Application;
using XCloud.Core.Dto;
using XCloud.Platform.Auth.Application.Admin;
using XCloud.Platform.Common.Application.Service.Menu;
using XCloud.Platform.Core.Domain.Menu;
using XCloud.Platform.Framework.Controller;

namespace XCloud.Platform.Api.Controller.Admin;

[Route("/api/sys/menu")]
public class SysMenuController : PlatformBaseController, IAdminController
{
    private readonly IMenuService _menuService;
    public SysMenuController(IMenuService menuService)
    {
        this._menuService = menuService;
    }

    [HttpPost("tree")]
    public async Task<ApiResponse<IEnumerable<AntDesignTreeNode>>> QueryTree([FromBody] IdDto dto)
    {
        var admin = await this.GetRequiredAuthedAdminAsync();

        var menus = await this._menuService.QueryMenuByGroupAsync(dto.Id);

        var tree = menus.BuildAntTree(title_selector: x => x.Name);

        return new ApiResponse<IEnumerable<AntDesignTreeNode>>().SetData(tree);
    }

    [HttpPost("save")]
    public async Task<ApiResponse<object>> SaveMenu([FromBody] SysMenu data)
    {
        data.Should().NotBeNull();

        var admin = await this.GetRequiredAuthedAdminAsync();

        if (string.IsNullOrWhiteSpace(data.Id))
        {
            var res = await this._menuService.AddMenuAsync(data);
            res.ThrowIfErrorOccured();
        }
        else
        {
            await this._menuService.UpdateMenuAsync(data);
        }
        return new ApiResponse<object>();
    }

    [HttpPost("delete")]
    public async Task<ApiResponse<object>> DeleteMenu([FromBody] IdDto dto)
    {
        var admin = await this.GetRequiredAuthedAdminAsync();

        var success = await this._menuService.DeleteMenuWhenNoChildrenAsync(dto.Id);

        if (!success)
        {
            throw new UserFriendlyException("删除失败");
        }

        return new ApiResponse<object>();
    }
}