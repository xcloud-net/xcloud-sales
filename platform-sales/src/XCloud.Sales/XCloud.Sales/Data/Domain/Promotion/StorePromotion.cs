using XCloud.Sales.Core;

namespace XCloud.Sales.Data.Domain.Promotion;

public class StorePromotion : SalesBaseEntity<string>, ISoftDelete, IHasCreationTime
{
    public StorePromotion()
    {
        //
    }

    public string Name { get; set; }
    public string Description { get; set; }
    public bool IsExclusive { get; set; }
    public string Condition { get; set; }
    public string Result { get; set; }
    public int Order { get; set; }
    public DateTime? StartTime { get; set; }
    public DateTime? EndTime { get; set; }
    public bool IsActive { get; set; }
    public bool IsDeleted { get; set; }
    public DateTime CreationTime { get; set; }
}