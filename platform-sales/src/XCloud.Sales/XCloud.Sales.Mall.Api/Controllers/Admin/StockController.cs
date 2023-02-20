using Microsoft.AspNetCore.Mvc;
using XCloud.Core.Dto;
using XCloud.Sales.Services.Stock;

namespace XCloud.Sales.Mall.Api.Controllers.Admin;

[StoreAuditLog]
[Route("api/mall-admin/stock")]
public class StockController : ShopBaseController
{
    private readonly IWarehouseStockService _warehouseStockService;

    public StockController(IWarehouseStockService warehouseStockService)
    {
        this._warehouseStockService = warehouseStockService;
    }

    [HttpPost("paging")]
    public async Task<PagedResponse<WarehouseStockDto>> StockPagingAsync([FromBody] QueryWarehouseStockInput dto)
    {
        var storeAdministrator = await this.StoreAuthService.GetRequiredStoreAdministratorAsync();

        await this.SalesPermissionService.CheckRequiredPermissionAsync(storeAdministrator,
            SalesPermissions.ManageStock);

        var response = await this._warehouseStockService.QueryPagingAsync(dto);

        if (response.IsNotEmpty)
        {
            await this._warehouseStockService.AttachDataAsync(response.Items.ToArray(),
                new AttachWarehouseStockDataInput() { Items = true });
            var items = response.Items.SelectMany(x => x.Items).WhereNotNull().ToArray();
            await this._warehouseStockService.AttachDataAsync(items,
                new AttachWarehouseStockItemDataInput() { Goods = true });
        }

        return response;
    }

    [HttpPost("item-paging")]
    public async Task<PagedResponse<WarehouseStockItemDto>> StockItemPagingAsync(
        [FromBody] QueryWarehouseStockItemInput dto)
    {
        var storeAdministrator = await this.StoreAuthService.GetRequiredStoreAdministratorAsync();

        await this.SalesPermissionService.CheckRequiredPermissionAsync(storeAdministrator,
            SalesPermissions.ManageStock);

        var response = await this._warehouseStockService.QueryItemPagingAsync(dto);

        if (response.IsNotEmpty)
        {
            await this._warehouseStockService.AttachDataAsync(response.Items.ToArray(),
                new AttachWarehouseStockItemDataInput()
                {
                    Goods = true,
                    WarehouseStock = true
                });
            
            var stocks = response.Items
                .Where(x => x.WarehouseStock != null)
                .Select(x => x.WarehouseStock).ToArray();
            await this._warehouseStockService.AttachDataAsync(stocks, new AttachWarehouseStockDataInput()
            {
                Warehouse = true,
                Supplier = true
            });
        }

        return response;
    }

    [HttpPost("approve")]
    public async Task<ApiResponse<object>> ApproveStockAsync([FromBody] IdDto dto)
    {
        var storeAdministrator = await this.StoreAuthService.GetRequiredStoreAdministratorAsync();

        await this.SalesPermissionService.CheckRequiredPermissionAsync(storeAdministrator,
            SalesPermissions.ManageStock);

        await this._warehouseStockService.ApproveStockAsync(dto.Id);

        return new ApiResponse<object>();
    }

    [HttpPost("delete-unapproved-stock")]
    public async Task<ApiResponse<object>> DeleteUnApprovedStockAsync([FromBody] IdDto dto)
    {
        var storeAdministrator = await this.StoreAuthService.GetRequiredStoreAdministratorAsync();

        await this.SalesPermissionService.CheckRequiredPermissionAsync(storeAdministrator,
            SalesPermissions.ManageStock);

        await this._warehouseStockService.DeleteUnApprovedStockAsync(dto.Id);

        return new ApiResponse<object>();
    }

    [HttpPost("insert-stock")]
    public async Task<ApiResponse<object>> InsertStockAsync([FromBody] WarehouseStockDto dto)
    {
        var storeAdministrator = await this.StoreAuthService.GetRequiredStoreAdministratorAsync();

        await this.SalesPermissionService.CheckRequiredPermissionAsync(storeAdministrator,
            SalesPermissions.ManageStock);

        await this._warehouseStockService.InsertWarehouseStockAsync(dto);

        return new ApiResponse<object>();
    }
}