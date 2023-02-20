using Microsoft.AspNetCore.Mvc;
using XCloud.Sales.Services.Stock;
using XCloud.Core.Dto;

namespace XCloud.Sales.Mall.Api.Controllers.Admin;

[StoreAuditLog]
[Route("api/mall-admin/supplier")]
public class SupplierController: ShopBaseController
{
    private readonly ISupplierService _supplierService;

    public SupplierController(ISupplierService supplierService)
    {
        _supplierService = supplierService;
    }
    
    [HttpPost("save")]
    public async Task<ApiResponse<object>> SaveAsync([FromBody] SupplierDto dto)
    {
        var storeAdministrator = await this.StoreAuthService.GetRequiredStoreAdministratorAsync();

        await this.SalesPermissionService.CheckRequiredPermissionAsync(storeAdministrator,
            SalesPermissions.ManageStock);

        if (string.IsNullOrWhiteSpace(dto.Id))
        {
            await this._supplierService.InsertAsync(dto);
        }
        else
        {
            await this._supplierService.UpdateAsync(dto);
        }

        return new ApiResponse<object>();
    }

    [HttpPost("update-status")]
    public async Task<ApiResponse<object>> UpdateStatusAsync([FromBody] UpdateSupplierStatusInput dto)
    {
        var storeAdministrator = await this.StoreAuthService.GetRequiredStoreAdministratorAsync();

        await this.SalesPermissionService.CheckRequiredPermissionAsync(storeAdministrator,
            SalesPermissions.ManageStock);

        await this._supplierService.UpdateStatusAsync(dto);
        
        return new ApiResponse<object>();
    }

    [Obsolete]
    [HttpPost("paging")]
    public async Task<PagedResponse<SupplierDto>> QueryPagingAsync([FromBody] QuerySupplierPagingInput dto)
    {
        var storeAdministrator = await this.StoreAuthService.GetRequiredStoreAdministratorAsync();

        await this.SalesPermissionService.CheckRequiredPermissionAsync(storeAdministrator,
            SalesPermissions.ManageStock);

        var response = await this._supplierService.QueryPagingAsync(dto);

        return response;
    }
    
    [HttpPost("all")]
    public async Task<ApiResponse<SupplierDto[]>> QueryAllAsync()
    {
        var storeAdministrator = await this.StoreAuthService.GetRequiredStoreAdministratorAsync();

        await this.SalesPermissionService.CheckRequiredPermissionAsync(storeAdministrator,
            SalesPermissions.ManageStock);

        var response = await this._supplierService.QueryAllAsync();

        return new ApiResponse<SupplierDto[]>(response);
    }
}