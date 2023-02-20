using XCloud.Sales.Core;

namespace XCloud.Sales.Data.Domain.Catalog;

public class TagGoods : SalesBaseEntity
{
    public string TagId { get; set; }
    public int GoodsId { get; set; }
}