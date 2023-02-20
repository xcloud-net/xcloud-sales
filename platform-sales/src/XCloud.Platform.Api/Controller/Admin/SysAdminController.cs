using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using XCloud.AspNetMvc.ModelBinder.JsonModel;
using XCloud.Core.Cache;
using XCloud.Core.Dto;
using XCloud.Platform.Auth.Application.Admin;
using XCloud.Platform.Core.Domain.Admin;
using XCloud.Platform.Framework.Controller;
using XCloud.Platform.Member.Application.Service.Admin;
using XCloud.Platform.Member.Application.Service.AdminPermission;

namespace XCloud.Platform.Api.Controller.Admin;

[Route("/api/sys/admin")]
public class SysAdminController : PlatformBaseController, IAdminController
{
    private readonly IAdminService _adminService;
    private readonly IAdminAccountService _adminAccountService;
    private readonly IAdminRoleService _roleService;
    private readonly IAdminPermissionService _adminPermissionService;

    public SysAdminController(
        IAdminService userService,
        IAdminAccountService adminAccountService,
        IAdminRoleService roleService,
        IAdminPermissionService adminPermissionService)
    {
        this._adminService = userService;
        this._adminAccountService = adminAccountService;
        this._roleService = roleService;
        _adminPermissionService = adminPermissionService;
    }

    /// <summary>
    /// 个人资料
    /// </summary>
    /// <returns></returns>
    [HttpPost("profile")]
    public async Task<ApiResponse<SysAdminDto>> Profile()
    {
        var loginAdmin = await this.GetRequiredAuthedAdminAsync();

        var data = await this._adminService.GetAdminProfileByIdAsync(
            new IdDto(loginAdmin.Id),
            new CachePolicy() { Cache = true });

        if (data == null)
            return new ApiResponse<SysAdminDto>().SetError("err");

        return new ApiResponse<SysAdminDto>().SetData(data);
    }
    
    [HttpPost("permissions")]
    public async Task<GrantedPermissionResponse> QueryAdminPermissionsAsync()
    {
        var loginAdmin = await this.GetRequiredAuthedAdminAsync();

        var response = await this._adminPermissionService.GetGrantedPermissionsAsync(new GetGrantedPermissionInput(loginAdmin.Id),
            new CachePolicy() { Cache = true });

        return response;
    }

    [HttpPost("list")]
    public async Task<PagedResponse<SysAdminDto>> QueryAdminPagingAsync([JsonData] QueryAdminDto filter)
    {
        filter.PageSize = 20;

        var loginAdmin = await this.GetRequiredAuthedAdminAsync();

        var data = await this._adminService.QueryAdminPagingAsync(filter);

        if (data.IsEmpty)
            return data;

        await this._roleService.AttachRolesAsync(data.Items.ToArray());

        var roles = data.Items.SelectMany(x => x.Roles).ToArray();

        await this._roleService.AttachPermissionAsync(roles);

        return data;
    }

    [HttpPost("add")]
    public async Task<ApiResponse<SysAdmin>> AddAdmin([JsonData] CreateAdminDto model)
    {
        var loginAdmin = await this.GetRequiredAuthedAdminAsync();

        var res = await this._adminAccountService.CreateAdminAccountAsync(model);

        return res;
    }

    [HttpPost("update-status")]
    public async Task<ApiResponse<object>> UpdateStatusAsync([JsonData] UpdateAdminStatusDto model)
    {
        var loginAdmin = await this.GetRequiredAuthedAdminAsync();

        await this._adminAccountService.UpdateAdminStatusAsync(model);

        return new ApiResponse<object>();
    }
}