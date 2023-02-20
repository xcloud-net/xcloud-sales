using Volo.Abp.Application.Dtos;
using XCloud.Core.Dto;
using XCloud.Sales.Data.Domain.Shipping;
using XCloud.Sales.Services.Orders;

namespace XCloud.Sales.Services.Shipping;

public class ShipmentOrderItemDto : ShipmentOrderItem, IEntityDto
{
    public OrderItemDto OrderItem { get; set; }
}

public class ShipmentDto : Shipment, IEntityDto
{
    public ShipmentOrderItemDto[] Items { get; set; }
}

public class QueryShipmentInput : PagedRequest, IEntityDto
{
    public string OrderId { get; set; }
}

public class AttachDataInput : IEntityDto
{
    public bool Items { get; set; } = false;
}

public class AttachItemDataInput : IEntityDto
{
    public bool OrderItem { get; set; } = false;
}