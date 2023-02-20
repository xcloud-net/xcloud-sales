using XCloud.Sales.Core;

namespace XCloud.Sales.Data.Domain.Orders;

public class OrderItem : SalesBaseEntity
{
    public string OrderId { get; set; }

    public int GoodsId { get; set; }

    public int GoodsSpecCombinationId { get; set; }

    public string GoodsName { get; set; }

    public int Quantity { get; set; }

    public decimal UnitPrice { get; set; }

    public decimal Price { get; set; }

    public decimal GradePriceOffset { get; set; }

    public decimal? ItemWeight { get; set; }
}