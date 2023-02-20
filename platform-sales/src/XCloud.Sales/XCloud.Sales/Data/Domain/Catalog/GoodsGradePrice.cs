using XCloud.Sales.Core;

namespace XCloud.Sales.Data.Domain.Catalog;

public class GoodsGradePrice : SalesBaseEntity<string>, IHasCreationTime
{
    public int GoodsId { get; set; }

    public int GoodsCombinationId { get; set; }
        
    public string GradeId { get; set; }
        
    public decimal Price { get; set; }

    public DateTime CreationTime { get; set; }
}