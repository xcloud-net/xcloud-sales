using Microsoft.AspNetCore.Mvc;
using XCloud.Core.Dto;
using XCloud.Sales.Services.Orders;
using XCloud.Sales.Services.Shipping;

namespace XCloud.Sales.Mall.Api.Controllers.Admin;

[StoreAuditLog]
[Route("api/mall-admin/shipping")]
public class ShippingController : ShopBaseController
{
    private readonly IOrderService _orderService;
    private readonly IOrderProcessingService _orderProcessingService;
    private readonly IShipmentService _shipmentService;

    public ShippingController(IOrderService orderService,
        IOrderProcessingService orderProcessingService,
        IShipmentService shipmentService)
    {
        this._shipmentService = shipmentService;
        this._orderService = orderService;
        this._orderProcessingService = orderProcessingService;
    }

    [HttpPost("by-order-id")]
    public async Task<ApiResponse<ShipmentDto[]>> ShipmentByOrderId([FromBody] IdDto dto)
    {
        var storeAdministrator = await this.StoreAuthService.GetRequiredStoreAdministratorAsync();

        await this.SalesPermissionService.CheckRequiredPermissionAsync(storeAdministrator,
            SalesPermissions.ManageOrders);

        var response = await this._shipmentService.QueryByOrderIdAsync(dto.Id);

        await this._shipmentService.AttachDataAsync(response, new AttachDataInput() { Items = true });

        var items = response.SelectMany(x => x.Items).ToArray();
        await this._shipmentService.AttachItemDataAsync(items, new AttachItemDataInput() { OrderItem = true });

        var orderItems = items.Select(x => x.OrderItem).WhereNotNull().ToArray();
        await this._orderService.AttachOrderItemDataAsync(orderItems, new OrderItemAttachDataInput() { Goods = true });

        return new ApiResponse<ShipmentDto[]>(response);
    }

    [HttpPost("ship-order")]
    public async Task<ApiResponse<ShipmentDto>> CreateAsync([FromBody] ShipmentDto dto)
    {
        var storeAdministrator = await this.StoreAuthService.GetRequiredStoreAdministratorAsync();

        await this.SalesPermissionService.CheckRequiredPermissionAsync(storeAdministrator,
            SalesPermissions.ManageOrders);

        var response = await this._shipmentService.CreateShippingAsync(dto);
        response.ThrowIfErrorOccured();

        await this._orderProcessingService.MarkAsShippedAsync(new IdDto() { Id = dto.OrderId });

        return response;
    }
}