using XCloud.Sales.Core;

namespace XCloud.Sales.Data.Domain.Users;

public class PrepaidCard : SalesBaseEntity<string>, IHasCreationTime, ISoftDelete
{
    public decimal Amount { get; set; }
    public DateTime? EndTime { get; set; }
    public int UserId { get; set; }
    public bool Used { get; set; }
    public DateTime? UsedTime { get; set; }

    public bool IsActive { get; set; }
    public bool IsDeleted { get; set; }

    public DateTime CreationTime { get; set; }
}