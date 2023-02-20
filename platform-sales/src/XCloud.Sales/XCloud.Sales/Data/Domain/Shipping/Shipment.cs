using XCloud.Sales.Core;

namespace XCloud.Sales.Data.Domain.Shipping;

/// <summary>
/// Represents a shipment
/// </summary>
public class Shipment : SalesBaseEntity<string>
{
    public string OrderId { get; set; }

    public string ShippingMethod { get; set; }

    public string ExpressName { get; set; }

    public string TrackingNumber { get; set; }

    public decimal? TotalWeight { get; set; }

    public DateTime? ShippedTime { get; set; }

    public DateTime? DeliveryTime { get; set; }

    public DateTime CreationTime { get; set; }
}