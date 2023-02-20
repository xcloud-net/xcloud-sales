using Microsoft.EntityFrameworkCore.Metadata.Builders;
using XCloud.Sales.Data.Domain.Aftersale;

namespace XCloud.Sales.Data.Mapping.Aftersale;

public class AfterSalesItemMap : SalesEntityTypeConfiguration<AftersalesItem>
{
    public override void Configure(EntityTypeBuilder<AftersalesItem> builder)
    {
        builder.ToTable(nameof(AftersalesItem));

        builder.Property(x => x.AftersalesId).IsRequired().HasMaxLength(100);
        builder.Property(x => x.OrderId).IsRequired().HasMaxLength(100);
        builder.Property(x => x.OrderItemId);
        builder.Property(x => x.Quantity);

        base.Configure(builder);
    }
}