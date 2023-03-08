using Microsoft.EntityFrameworkCore.Metadata.Builders;
using XCloud.Sales.Data.Domain.Finance;

namespace XCloud.Sales.Data.Mapping.Finance;

public class OrderRefundBillMap : SalesEntityTypeConfiguration<OrderRefundBill>
{
    public override void Configure(EntityTypeBuilder<OrderRefundBill> builder)
    {
        builder.ToTable(nameof(OrderRefundBill));

        builder.HasKey(x => x.Id);

        builder.Property(x => x.OrderId).IsRequired().HasMaxLength(100);
        builder.Property(x => x.BillId).IsRequired().HasMaxLength(100);
        builder.Property(x => x.Price).HasPrecision(18, 2);

        builder.Property(x => x.Approved).HasDefaultValue(false);
        builder.Property(x => x.ApprovedTime);

        builder.Property(x => x.Refunded);
        builder.Property(x => x.RefundTime);
        builder.Property(x => x.RefundTransactionId).HasMaxLength(200);
        builder.Property(x => x.RefundNotifyData);

        builder.Property(x => x.LastModificationTime);

        builder.Property(x => x.IsDeleted);
        builder.Property(x => x.DeletionTime);

        builder.Property(x => x.CreationTime);

        base.Configure(builder);
    }
}