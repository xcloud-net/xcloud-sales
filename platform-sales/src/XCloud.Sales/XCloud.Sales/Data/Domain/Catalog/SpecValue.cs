using XCloud.Sales.Core;

namespace XCloud.Sales.Data.Domain.Catalog;

public class SpecValue : SalesBaseEntity, ISoftDelete
{
    public int GoodsSpecId { get; set; }
    
    public string Name { get; set; }

    public decimal PriceOffset { get; set; }

    public bool IsDeleted { get; set; }
}