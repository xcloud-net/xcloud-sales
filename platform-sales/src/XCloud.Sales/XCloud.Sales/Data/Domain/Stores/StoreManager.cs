using XCloud.Sales.Core;

namespace XCloud.Sales.Data.Domain.Stores;

public class StoreManager : SalesBaseEntity<string>, IHasCreationTime, ISoftDelete
{
    public string StoreId { get; set; }
    public string GlobalUserId { get; set; }
    public string NickName { get; set; }
    public string Avatar { get; set; }
    public int Role { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreationTime { get; set; }
    public bool IsDeleted { get; set; }
}