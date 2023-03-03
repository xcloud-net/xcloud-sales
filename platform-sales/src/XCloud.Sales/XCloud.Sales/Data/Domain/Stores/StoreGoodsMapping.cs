using XCloud.Sales.Core;

namespace XCloud.Sales.Data.Domain.Stores;

public class StoreGoodsMapping : SalesBaseEntity<string>,IHasCreationTime,IHasModificationTime
{
    public string StoreId { get; set; }
    
    public int GoodsCombinationId { get; set; }

    public decimal? Price { get; set; }

    public int StockQuantity { get; set; }
    
    public DateTime CreationTime { get; set; }
    
    public DateTime? LastModificationTime { get; set; }
}