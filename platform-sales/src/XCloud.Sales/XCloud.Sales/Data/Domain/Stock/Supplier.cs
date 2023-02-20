using XCloud.Sales.Core;

namespace XCloud.Sales.Data.Domain.Stock;

public class Supplier : SalesBaseEntity<string>, IHasCreationTime, ISoftDelete
{
    public string Name { get; set; }
    public string ContactName { get; set; }
    public string Telephone { get; set; }
    public string Address { get; set; }
    public DateTime CreationTime { get; set; }
    public bool IsDeleted { get; set; }
}