using XCloud.Sales.Core;

namespace XCloud.Sales.Data.Domain.Common;

public class Pages : SalesBaseEntity<string>, IHasCreationTime, ISoftDelete
{
    public string SeoName { get; set; }

    public string CoverImageResourceData { get; set; }

    public string Title { get; set; }

    public string Description { get; set; }

    public string BodyContent { get; set; }

    public int ReadCount { get; set; }

    public bool IsPublished { get; set; }

    public DateTime? PublishedTime { get; set; }

    public DateTime CreationTime { get; set; }

    public bool IsDeleted { get; set; }
}