using Microsoft.AspNetCore.Mvc;
using XCloud.Core.Dto;
using XCloud.Sales.Data.Domain.Users;
using XCloud.Sales.Services.Users;

namespace XCloud.Sales.Mall.Api.Controllers.Admin;

[StoreAuditLog]
[Route("api/mall-admin/prepaid-card")]
public class PrepaidCardController : ShopBaseController
{
    private readonly IPrepaidCardService _prepaidCardService;
    public PrepaidCardController(IPrepaidCardService prepaidCardService)
    {
        this._prepaidCardService = prepaidCardService;
    }

    [HttpPost("unused-total")]
    public async Task<ApiResponse<decimal>> QueryUnusedTotalAsync()
    {
        var storeAdministrator = await this.StoreAuthService.GetRequiredStoreAdministratorAsync();

        await this.SalesPermissionService.CheckRequiredPermissionAsync(storeAdministrator,
            SalesPermissions.ManageFinance);

        var total = await this._prepaidCardService.QueryUnusedAmountTotalAsync();

        return new ApiResponse<decimal>(total);
    }

    [HttpPost("paging")]
    public async Task<PagedResponse<PrepaidCardDto>> QueryPagingAsync([FromBody] QueryPrepaidCardPagingInput dto)
    {
        var storeAdministrator = await this.StoreAuthService.GetRequiredStoreAdministratorAsync();

        await this.SalesPermissionService.CheckRequiredPermissionAsync(storeAdministrator,
            SalesPermissions.ManageFinance);

        var response = await this._prepaidCardService.QueryPagingAsync(dto);

        return response;
    }

    [HttpPost("create")]
    public async Task<ApiResponse<PrepaidCard>> CreateAsync([FromBody] PrepaidCardDto dto)
    {
        var storeAdministrator = await this.StoreAuthService.GetRequiredStoreAdministratorAsync();

        await this.SalesPermissionService.CheckRequiredPermissionAsync(storeAdministrator,
            SalesPermissions.ManageFinance);

        var entity = await this._prepaidCardService.CreatePrepaidCardAsync(dto);

        return new ApiResponse<PrepaidCard>(entity);
    }

    [HttpPost("update-status")]
    public async Task<ApiResponse<object>> UpdateStatusAsync([FromBody] UpdatePrepaidCardStatusInput dto)
    {
        var storeAdministrator = await this.StoreAuthService.GetRequiredStoreAdministratorAsync();

        await this.SalesPermissionService.CheckRequiredPermissionAsync(storeAdministrator,
            SalesPermissions.ManageFinance);

        await this._prepaidCardService.UpdateStatusAsync(dto);

        return new ApiResponse<object>();
    }
}