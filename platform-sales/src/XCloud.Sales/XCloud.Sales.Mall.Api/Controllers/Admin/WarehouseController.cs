using Microsoft.AspNetCore.Mvc;
using XCloud.Core.Dto;
using XCloud.Sales.Service.Authentication;
using XCloud.Sales.Service.WarehouseStock;

namespace XCloud.Sales.Mall.Api.Controllers.Admin;

[StoreAuditLog]
[Route("api/mall-admin/warehouse")]
public class WarehouseController: ShopBaseController
{
    private readonly IWarehouseService _warehouseService;

    public WarehouseController(IWarehouseService warehouseService)
    {
        _warehouseService = warehouseService;
    }
    
    [HttpPost("save")]
    public async Task<ApiResponse<object>> SaveAsync([FromBody] WarehouseDto dto)
    {
        var storeAdministrator = await this.StoreAuthService.GetRequiredStoreAdministratorAsync();

        await this.SalesPermissionService.CheckRequiredPermissionAsync(storeAdministrator,
            SalesPermissions.ManageStock);

        if (string.IsNullOrWhiteSpace(dto.Id))
        {
            await this._warehouseService.InsertAsync(dto);
        }
        else
        {
            await this._warehouseService.UpdateAsync(dto);
        }

        return new ApiResponse<object>();
    }

    [HttpPost("update-status")]
    public async Task<ApiResponse<object>> UpdateStatusAsync([FromBody] UpdateWarehouseStatusInput dto)
    {
        var storeAdministrator = await this.StoreAuthService.GetRequiredStoreAdministratorAsync();

        await this.SalesPermissionService.CheckRequiredPermissionAsync(storeAdministrator,
            SalesPermissions.ManageStock);

        await this._warehouseService.UpdateStatusAsync(dto);
        
        return new ApiResponse<object>();
    }

    [Obsolete]
    [HttpPost("paging")]
    public async Task<PagedResponse<WarehouseDto>> QueryPagingAsync([FromBody] QueryWarehousePagingInput dto)
    {
        var storeAdministrator = await this.StoreAuthService.GetRequiredStoreAdministratorAsync();

        await this.SalesPermissionService.CheckRequiredPermissionAsync(storeAdministrator,
            SalesPermissions.ManageStock);

        var response = await this._warehouseService.QueryPagingAsync(dto);

        return response;
    }
    
    [HttpPost("all")]
    public async Task<ApiResponse<WarehouseDto[]>> QueryAllAsync()
    {
        var storeAdministrator = await this.StoreAuthService.GetRequiredStoreAdministratorAsync();

        await this.SalesPermissionService.CheckRequiredPermissionAsync(storeAdministrator,
            SalesPermissions.ManageStock);

        var response = await this._warehouseService.QueryAllAsync();

        return new ApiResponse<WarehouseDto[]>(response);
    }
}