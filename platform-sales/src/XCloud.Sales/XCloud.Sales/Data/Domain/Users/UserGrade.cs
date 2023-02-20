using XCloud.Sales.Core;

namespace XCloud.Sales.Data.Domain.Users;

public class UserGrade : SalesBaseEntity<string>, ISoftDelete
{
    public string Name { get; set; }

    public string Description { get; set; }
        
    public int UserCount { get; set; }
        
    public int Sort { get; set; }
        
    public bool IsDeleted { get; set; }
}