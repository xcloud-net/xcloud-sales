
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using XCloud.Sales.Data.Domain.Finance;

namespace XCloud.Sales.Data.Mapping.Finance;

public class OrderBillMap : SalesEntityTypeConfiguration<OrderBill>
{
    public override void Configure(EntityTypeBuilder<OrderBill> builder)
    {
        builder.ToTable(nameof(OrderBill));

        builder.HasKey(x => x.Id);

        builder.Property(x => x.OrderId).IsRequired().HasMaxLength(100);
        builder.Property(x => x.Price).HasPrecision(18, 2);

        builder.Property(x => x.PaymentMethod);

        builder.Property(x => x.Paid);
        builder.Property(x => x.PaymentTransactionId).HasMaxLength(200);
        builder.Property(x => x.PayTime);
        builder.Property(x => x.NotifyData);

        builder.Property(x => x.Refunded);
        builder.Property(x => x.RefundTime);
        builder.Property(x => x.RefundTransactionId).HasMaxLength(200);
        builder.Property(x => x.RefundNotifyData);

        builder.Property(x => x.IsDeleted);
        builder.Property(x => x.CreationTime);

        base.Configure(builder);
    }
}