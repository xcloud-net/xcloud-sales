using XCloud.Sales.Core;

namespace XCloud.Sales.Data.Domain.Coupons;

public class Coupon : SalesBaseEntity, IHasCreationTime, ISoftDelete
{
    public string Title { get; set; }
    public decimal Value { get; set; }
    public DateTime? StartTime { get; set; }
    public DateTime? EndTime { get; set; }
    public decimal MinimumConsumption { get; set; }
    public int? ExpiredDaysFromIssue { get; set; }

    public int Amount { get; set; }
    public bool IsAmountLimit { get; set; }
    public int? AccountIssuedLimitCount { get; set; }

    public int IssuedAmount { get; set; }
    public int UsedAmount { get; set; }

    public DateTime CreationTime { get; set; }
    public bool IsDeleted { get; set; }
}