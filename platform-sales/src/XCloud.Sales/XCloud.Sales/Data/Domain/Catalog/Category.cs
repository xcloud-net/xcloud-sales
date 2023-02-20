using XCloud.Sales.Core;

namespace XCloud.Sales.Data.Domain.Catalog;

public class Category : SalesBaseEntity, ISoftDelete, IHasDeletionTime
{
    public string Name { get; set; }

    public string SeoName { get; set; }

    public string Description { get; set; }

    public int RootId { get; set; }

    public string NodePath { get; set; }

    public int ParentCategoryId { get; set; }

    public int PictureId { get; set; }

    public string PriceRanges { get; set; }

    public bool ShowOnHomePage { get; set; }

    public bool Published { get; set; }

    public DateTime? DeletionTime { get; set; }

    public bool IsDeleted { get; set; }

    public int DisplayOrder { get; set; }

    public bool Recommend { get; set; }

    public DateTime CreationTime { get; set; }

    public DateTime LastModificationTime { get; set; }
}