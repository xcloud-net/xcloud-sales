using XCloud.Sales.Core;

namespace XCloud.Sales.Data.Domain.Catalog;

public class GoodsSpecCombination : SalesBaseEntity, ISoftDelete, IHasDeletionTime, IHasCreationTime,
    IHasModificationTime
{
    public string Name { get; set; }

    public string Description { get; set; }

    public string Sku { get; set; }

    public string Color { get; set; }

    public decimal CostPrice { get; set; }

    public decimal Price { get; set; }

    public decimal Weight { get; set; }

    public int StockQuantity { get; set; }

    public int PictureId { get; set; }

    public int GoodsId { get; set; }

    public string SpecificationsJson { get; set; }

    public bool IsActive { get; set; }

    public DateTime? DeletionTime { get; set; }
    public bool IsDeleted { get; set; }

    public DateTime CreationTime { get; set; }
    public DateTime? LastModificationTime { get; set; }
}