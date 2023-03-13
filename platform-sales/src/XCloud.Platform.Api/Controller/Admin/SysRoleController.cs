using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using XCloud.Core.Dto;
using XCloud.Platform.Application.Member.Service.Security;
using XCloud.Platform.Auth.Application.Admin;
using XCloud.Platform.Framework.Controller;

namespace XCloud.Platform.Api.Controller.Admin;

[Route("/api/sys/role")]
public class SysRoleController : PlatformBaseController, IAdminController
{
    private readonly IAdminRoleService _adminRoleService;
    private readonly IRoleService _roleService;
    private readonly IAdminSecurityService _securityService;

    public SysRoleController(IAdminRoleService adminRoleService, IAdminSecurityService securityService,
        IRoleService roleService)
    {
        this._adminRoleService = adminRoleService;
        _securityService = securityService;
        _roleService = roleService;
    }

    [HttpPost("paging")]
    public async Task<PagedResponse<SysRoleDto>> QueryPagingAsync([FromBody] QueryRolePagingInput dto)
    {
        var loginAdmin = await this.GetRequiredAuthedAdminAsync();

        var response = await this._roleService.QueryPagingAsync(dto);

        if (response.IsNotEmpty)
        {
            await this._roleService.AttachPermissionAsync(response.Items.ToArray());
        }

        return response;
    }

    [HttpPost("delete")]
    public async Task<ApiResponse<object>> DeleteAsync([FromBody] IdDto dto)
    {
        var loginAdmin = await this.GetRequiredAuthedAdminAsync();

        await this._roleService.DeleteByIdAsync(dto.Id);

        return new ApiResponse<object>();
    }

    [HttpPost("save")]
    public async Task<ApiResponse<object>> SaveAsync([FromBody] SysRoleDto dto)
    {
        var loginAdmin = await this.GetRequiredAuthedAdminAsync();

        if (string.IsNullOrWhiteSpace(dto.Id))
        {
            await this._roleService.InsertAsync(dto);
        }
        else
        {
            await this._roleService.UpdateAsync(dto);
        }

        return new ApiResponse<object>();
    }

    [HttpPost("save-admin-roles")]
    public async Task<ApiResponse<object>> SaveAdminRolesAsync([FromBody] AssignAdminRolesInput dto)
    {
        var loginAdmin = await this.GetRequiredAuthedAdminAsync();

        await this._adminRoleService.SetAdminRolesAsync(dto.Id, dto.RoleIds);

        return new ApiResponse<object>();
    }

    [HttpPost("save-role-permissions")]
    public async Task<ApiResponse<object>> SaveRolePermissionsAsync([FromBody] AssignPermissionToRoleInput dto)
    {
        var loginAdmin = await this.GetRequiredAuthedAdminAsync();

        await this._roleService.SetRolePermissionsAsync(dto.Id, dto.PermissionKeys);

        return new ApiResponse<object>();
    }
}