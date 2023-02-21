using XCloud.Sales.Core;

namespace XCloud.Sales.Data.Domain.WarehouseStock;

public class StockItem : SalesBaseEntity<string>, IHasModificationTime
{
    public string WarehouseStockId { get; set; }
    public int GoodsId { get; set; }
    public int CombinationId { get; set; }
    public int Quantity { get; set; }
    public decimal Price { get; set; }

    //
    public int DeductQuantity { get; set; }
    public bool RuningOut { get; set; }
    public DateTime? LastModificationTime { get; set; }
}