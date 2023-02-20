using XCloud.Sales.Core;

namespace XCloud.Sales.Data.Domain.Users;

public class Favorites : SalesBaseEntity
{
    public int GoodsId { get; set; }
    public int UserId { get; set; }
    public DateTime CreationTime { get; set; }
}