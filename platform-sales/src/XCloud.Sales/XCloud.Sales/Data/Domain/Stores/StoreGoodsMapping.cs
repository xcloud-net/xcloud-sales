using XCloud.Sales.Core;

namespace XCloud.Sales.Data.Domain.Stores;

public class StoreGoodsMapping : SalesBaseEntity<string>
{
    public string StoreId { get; set; }
    
    public int GoodsCombinationId { get; set; }
}