using XCloud.Sales.Core;

namespace XCloud.Sales.Data.Domain.Catalog;

public class GoodsCollection : SalesBaseEntity<string>, ISoftDelete, IHasCreationTime, IHasModificationTime, IHasDeletionTime
{
    public string Name { get; set; }

    public string Description { get; set; }

    public string Keywords { get; set; }

    public int ApplyedCount { get; set; }

    public bool IsDeleted { get; set; }

    public DateTime CreationTime { get; set; }

    public DateTime? LastModificationTime { get; set; }

    public DateTime? DeletionTime { get; set; }
}