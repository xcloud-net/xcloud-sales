using XCloud.Sales.Core;

namespace XCloud.Sales.Data.Domain.Stock;

public class Warehouse : SalesBaseEntity<string>, IHasCreationTime, ISoftDelete
{
    public string Name { get; set; }
    public string Address { get; set; }
    public double? Lat { get; set; }
    public double? Lng { get; set; }
    public DateTime CreationTime { get; set; }
    
    public bool IsDeleted { get; set; }
}