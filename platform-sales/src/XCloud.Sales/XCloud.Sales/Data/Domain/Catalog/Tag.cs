using XCloud.Sales.Core;

namespace XCloud.Sales.Data.Domain.Catalog;

public class Tag : SalesBaseEntity<string>, ISoftDelete
{
    public string Name { get; set; }
    public string Description { get; set; }
    public string Alert { get; set; }
    public string Link { get; set; }
    
    public bool IsDeleted { get; set; }
}