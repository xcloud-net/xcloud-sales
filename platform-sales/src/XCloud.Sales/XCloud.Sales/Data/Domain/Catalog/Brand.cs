using XCloud.Sales.Core;

namespace XCloud.Sales.Data.Domain.Catalog;

/// <summary>
/// Represents a brand
/// </summary>
public class Brand : SalesBaseEntity, ISoftDelete, IHasDeletionTime
{
    /// <summary>
    /// Gets or sets the name
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// Gets or sets the description
    /// </summary>
    public string Description { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether show on public  page 
    /// </summary>
    public bool ShowOnPublicPage { get; set; }

    /// <summary>
    /// Gets or sets the meta keywords
    /// </summary>
    public string MetaKeywords { get; set; }

    /// <summary>
    /// Gets or sets the meta description
    /// </summary>
    public string MetaDescription { get; set; }

    /// <summary>
    /// Gets or sets the meta title
    /// </summary>
    public string MetaTitle { get; set; }

    public int PictureId { get; set; }

    /// <summary>
    /// Gets or sets the page size
    /// </summary>
    public int PageSize { get; set; }

    /// <summary>
    /// Gets or sets the available price ranges
    /// </summary>
    public string PriceRanges { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the entity is published
    /// </summary>
    public bool Published { get; set; }

    public DateTime? DeletionTime { get; set; }
    public bool IsDeleted { get; set; }

    /// <summary>
    /// Gets or sets the display order
    /// </summary>
    public int DisplayOrder { get; set; }

    /// <summary>
    /// Gets or sets the date and time of instance creation
    /// </summary>
    public DateTime CreationTime { get; set; }

    /// <summary>
    /// Gets or sets the date and time of instance update
    /// </summary>
    public DateTime LastModificationTime { get; set; }
}