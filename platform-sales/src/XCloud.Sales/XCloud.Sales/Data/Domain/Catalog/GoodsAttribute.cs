using XCloud.Sales.Core;

namespace XCloud.Sales.Data.Domain.Catalog;

public class GoodsAttribute : SalesBaseEntity<string>, IHasCreationTime
{
    public int GoodsId { get; set; }
    public string Name { get; set; }
    public string Value { get; set; }
    public DateTime CreationTime { get; set; }
}