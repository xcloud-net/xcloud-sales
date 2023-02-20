using XCloud.Sales.Core;

namespace XCloud.Sales.Data.Domain.Catalog;

public class GoodsPriceHistory : SalesBaseEntity, IHasCreationTime
{
    public int GoodsId { get; set; }

    public int GoodsSpecCombinationId { get; set; }

    public decimal PreviousPrice { get; set; }

    public decimal Price { get; set; }

    public string Comment { get; set; }

    public DateTime CreationTime { get; set; }
}