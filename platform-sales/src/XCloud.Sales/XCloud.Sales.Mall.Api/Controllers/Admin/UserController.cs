using Microsoft.AspNetCore.Mvc;
using XCloud.Core.Dto;
using XCloud.Sales.Service.Authentication;
using XCloud.Sales.Service.Users;

namespace XCloud.Sales.Mall.Api.Controllers.Admin;

[StoreAuditLog]
[Route("api/mall-admin/user")]
public class UserController : ShopBaseController
{
    private readonly IUserService _userService;
    public UserController(IUserService userService)
    {
        this._userService = userService;
    }

    [HttpPost("update-status")]
    public async Task<ApiResponse<object>> UpdateStatusAsync([FromBody] UpdateUserStatusInput dto)
    {            
        var storeAdministrator = await this.StoreAuthService.GetRequiredStoreAdministratorAsync();

        await this.SalesPermissionService.CheckRequiredPermissionAsync(storeAdministrator,
            SalesPermissions.ManageUsers);

        await this._userService.UpdateStatusAsync(dto);

        await this.CurrentUnitOfWork.SaveChangesAsync();
        await this.EventBusService.NotifyRefreshUserInfoAsync(dto.Id);
        return new ApiResponse<object>();
    }

    [HttpPost("paging")]
    public async Task<PagedResponse<StoreUserDto>> QueryPaging([FromBody] QueryUserInput dto)
    {
        var storeAdministrator = await this.StoreAuthService.GetRequiredStoreAdministratorAsync();

        await this.SalesPermissionService.CheckRequiredPermissionAsync(storeAdministrator,
            SalesPermissions.ManageUsers);

        var response = await this._userService.QueryStoreUserPagingAsync(dto);

        return response;
    }
}