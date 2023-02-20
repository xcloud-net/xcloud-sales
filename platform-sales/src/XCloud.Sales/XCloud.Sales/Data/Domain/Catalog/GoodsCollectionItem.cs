using XCloud.Sales.Core;

namespace XCloud.Sales.Data.Domain.Catalog;

public class GoodsCollectionItem : SalesBaseEntity<string>, IHasCreationTime
{
    public string CollectionId { get; set; }
    public int GoodsId { get; set; }
    public int GoodsSpecCombinationId { get; set; }
    public int Quantity { get; set; }

    public DateTime CreationTime { get; set; }
}