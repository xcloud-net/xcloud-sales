using XCloud.Sales.Core;

namespace XCloud.Sales.Data.Domain.Orders;

public class Order : SalesBaseEntity<string>, ISoftDelete, IHasModificationTime
{
    public string StoreId { get; set; }
    public string OrderSn { get; set; }

    public int UserId { get; set; }

    public bool ShippingRequired { get; set; }
    public DateTime? ShippingTime { get; set; }
    public string ShippingAddressId { get; set; }
    public string ShippingAddressProvice { get; set; }
    public string ShippingAddressCity { get; set; }
    public string ShippingAddressArea { get; set; }
    public string ShippingAddressDetail { get; set; }
    public string ShippingAddressContactName { get; set; }
    public string ShippingAddressContact { get; set; }

    public int OrderStatusId { get; set; }
    public int ShippingStatusId { get; set; }
    public int PaymentStatusId { get; set; }

    public bool IsAftersales { get; set; }
    public string AfterSalesId { get; set; }

    public string GradeId { get; set; }
    public decimal GradePriceOffsetTotal { get; set; }

    public decimal OrderTotal { get; set; }
    public decimal OrderSubtotal { get; set; }
    public decimal OrderShippingFee { get; set; }

    public int? CouponId { get; set; }
    public decimal CouponDiscount { get; set; }

    public string PromotionId { get; set; }
    public decimal PromotionDiscount { get; set; }

    public decimal ExchangePointsAmount { get; set; }
    public decimal RefundedAmount { get; set; }
    [Obsolete]
    public int? RewardPointsHistoryId { get; set; }

    public string Remark { get; set; }
    public int? AffiliateId { get; set; }
    public string OrderIp { get; set; }

    public DateTime? PaidTime { get; set; }
    public bool OverPaid { get; set; }

    public bool IsDeleted { get; set; }
    public DateTime CreationTime { get; set; }

    public bool HideForAdmin { get; set; }
        
    public DateTime? LastModificationTime { get; set; }
}