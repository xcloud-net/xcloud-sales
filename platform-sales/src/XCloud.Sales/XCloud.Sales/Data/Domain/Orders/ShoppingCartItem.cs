using XCloud.Sales.Core;

namespace XCloud.Sales.Data.Domain.Orders;

public class ShoppingCartItem : SalesBaseEntity
{
    public int UserId { get; set; }

    public int GoodsId { get; set; }

    public int GoodsSpecCombinationId { get; set; }

    public int Quantity { get; set; }

    public DateTime CreationTime { get; set; }

    public DateTime LastModificationTime { get; set; }
}