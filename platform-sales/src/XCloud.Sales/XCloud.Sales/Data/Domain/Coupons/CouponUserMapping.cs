using XCloud.Sales.Core;

namespace XCloud.Sales.Data.Domain.Coupons;

/// <summary>
/// 优惠价用户映射
/// </summary>
public class CouponUserMapping : SalesBaseEntity, IHasCreationTime
{
    /// <summary>
    /// 客户id
    /// </summary>
    public int UserId { get; set; }

    /// <summary>
    /// 优惠券Id
    /// </summary>
    public int CouponId { get; set; }

    /// <summary>
    /// 是否已经使用
    /// </summary>
    public bool IsUsed { get; set; }

    /// <summary>
    /// 使用时间
    /// </summary>
    public DateTime? UsedTime { get; set; }

    public decimal Value { get; set; }

    public decimal MinimumConsumption { get; set; }

    public DateTime? ExpiredAt { get; set; }

    /// <summary>
    /// 兑换时间
    /// </summary>
    public DateTime CreationTime { get; set; }
}