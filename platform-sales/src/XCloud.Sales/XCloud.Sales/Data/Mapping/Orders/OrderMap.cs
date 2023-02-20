using Microsoft.EntityFrameworkCore.Metadata.Builders;
using XCloud.Sales.Data.Domain.Orders;

namespace XCloud.Sales.Data.Mapping.Orders;

public class OrderMap : SalesEntityTypeConfiguration<Order>
{
    public override void Configure(EntityTypeBuilder<Order> builder)
    {
        builder.ToTable(nameof(Order));

        builder.HasKey(o => o.Id);
        builder.Property(x => x.UserId).IsRequired().HasMaxLength(100);
        builder.Property(x => x.HideForAdmin).IsRequired().HasDefaultValue(false);

        builder.Property(x => x.StoreId).HasMaxLength(100);
        builder.Property(x => x.OrderSn).HasMaxLength(100);
        builder.Property(x => x.Remark).HasMaxLength(1000);
        builder.Property(x => x.OrderIp).HasMaxLength(100);
        builder.Property(x => x.AffiliateId);

        builder.Property(x => x.ShippingRequired);
        builder.Property(x => x.ShippingTime);
        builder.Property(x => x.ShippingAddressId).HasMaxLength(100);
        builder.Property(x => x.ShippingAddressProvice).HasMaxLength(100);
        builder.Property(x => x.ShippingAddressCity).HasMaxLength(100);
        builder.Property(x => x.ShippingAddressArea).HasMaxLength(100);
        builder.Property(x => x.ShippingAddressDetail).HasMaxLength(1000);
        builder.Property(x => x.ShippingAddressContactName).HasMaxLength(100);
        builder.Property(x => x.ShippingAddressContact).HasMaxLength(100);

        builder.Property(o => o.OrderShippingFee).HasPrecision(18, 2);
        builder.Property(o => o.OrderTotal).HasPrecision(18, 2);
        builder.Property(o => o.OrderSubtotal).HasPrecision(18, 2);
        builder.Property(o => o.RefundedAmount).HasPrecision(18, 2);
        builder.Property(x => x.GradeId).HasMaxLength(100);
        builder.Property(o => o.GradePriceOffsetTotal).HasPrecision(18, 2);

        builder.Property(x => x.CouponId);
        builder.Property(x => x.CouponDiscount).HasPrecision(18, 2);

        builder.Property(x => x.PromotionId).HasMaxLength(100);
        builder.Property(x => x.PromotionDiscount).HasPrecision(18, 2);

        builder.Property(x => x.IsAftersales);
        builder.Property(x => x.AfterSalesId).HasMaxLength(100);

        builder.Property(x => x.PaidTime);
        builder.Property(x => x.OverPaid);
        builder.Property(x => x.PaymentStatusId);
        builder.Property(x => x.OrderStatusId);
        builder.Property(x => x.ShippingStatusId);
        builder.Property(x => x.LastModificationTime);

        base.Configure(builder);
    }
}