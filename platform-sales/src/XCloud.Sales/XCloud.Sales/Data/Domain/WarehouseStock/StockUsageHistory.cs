using XCloud.Sales.Core;

namespace XCloud.Sales.Data.Domain.WarehouseStock;

public class StockUsageHistory : SalesBaseEntity<string>, IHasCreationTime
{
    public string OrderItemId { get; set; }
    public string WarehouseStockItemId { get; set; }
    public int Quantity { get; set; }
    public bool Revert { get; set; } = false;
    public DateTime? RevertTime { get; set; }
    public DateTime CreationTime { get; set; }
}