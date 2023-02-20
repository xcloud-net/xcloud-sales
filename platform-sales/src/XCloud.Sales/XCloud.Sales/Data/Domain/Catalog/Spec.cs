using XCloud.Sales.Core;

namespace XCloud.Sales.Data.Domain.Catalog;

public class Spec : SalesBaseEntity, ISoftDelete
{
    public string Name { get; set; }

    public int GoodsId { get; set; }

    public bool IsDeleted { get; set; }
}