using Microsoft.AspNetCore.Mvc;
using XCloud.Core.Dto;
using XCloud.Sales.Service.Authentication;
using XCloud.Sales.Service.WarehouseStock;

namespace XCloud.Sales.Mall.Api.Controllers.Admin;

[StoreAuditLog]
[Route("api/mall-admin/stock")]
public class StockController : ShopBaseController
{
    private readonly IStockService _stockService;

    public StockController(IStockService stockService)
    {
        this._stockService = stockService;
    }

    [HttpPost("paging")]
    public async Task<PagedResponse<StockDto>> StockPagingAsync([FromBody] QueryWarehouseStockInput dto)
    {
        var storeAdministrator = await this.StoreAuthService.GetRequiredStoreAdministratorAsync();

        await this.SalesPermissionService.CheckRequiredPermissionAsync(storeAdministrator,
            SalesPermissions.ManageStock);

        var response = await this._stockService.QueryPagingAsync(dto);

        if (response.IsNotEmpty)
        {
            await this._stockService.AttachDataAsync(response.Items.ToArray(),
                new AttachWarehouseStockDataInput() { Items = true });
            var items = response.Items.SelectMany(x => x.Items).WhereNotNull().ToArray();
            await this._stockService.AttachDataAsync(items,
                new AttachWarehouseStockItemDataInput() { Goods = true });
        }

        return response;
    }

    [HttpPost("item-paging")]
    public async Task<PagedResponse<StockItemDto>> StockItemPagingAsync(
        [FromBody] QueryWarehouseStockItemInput dto)
    {
        var storeAdministrator = await this.StoreAuthService.GetRequiredStoreAdministratorAsync();

        await this.SalesPermissionService.CheckRequiredPermissionAsync(storeAdministrator,
            SalesPermissions.ManageStock);

        var response = await this._stockService.QueryItemPagingAsync(dto);

        if (response.IsNotEmpty)
        {
            await this._stockService.AttachDataAsync(response.Items.ToArray(),
                new AttachWarehouseStockItemDataInput()
                {
                    Goods = true,
                    WarehouseStock = true
                });
            
            var stocks = response.Items
                .Where(x => x.Stock != null)
                .Select(x => x.Stock).ToArray();
            await this._stockService.AttachDataAsync(stocks, new AttachWarehouseStockDataInput()
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

        await this._stockService.ApproveStockAsync(dto.Id);

        return new ApiResponse<object>();
    }

    [HttpPost("delete-unapproved-stock")]
    public async Task<ApiResponse<object>> DeleteUnApprovedStockAsync([FromBody] IdDto dto)
    {
        var storeAdministrator = await this.StoreAuthService.GetRequiredStoreAdministratorAsync();

        await this.SalesPermissionService.CheckRequiredPermissionAsync(storeAdministrator,
            SalesPermissions.ManageStock);

        await this._stockService.DeleteUnApprovedStockAsync(dto.Id);

        return new ApiResponse<object>();
    }

    [HttpPost("insert-stock")]
    public async Task<ApiResponse<object>> InsertStockAsync([FromBody] StockDto dto)
    {
        var storeAdministrator = await this.StoreAuthService.GetRequiredStoreAdministratorAsync();

        await this.SalesPermissionService.CheckRequiredPermissionAsync(storeAdministrator,
            SalesPermissions.ManageStock);

        await this._stockService.InsertWarehouseStockAsync(dto);

        return new ApiResponse<object>();
    }
}