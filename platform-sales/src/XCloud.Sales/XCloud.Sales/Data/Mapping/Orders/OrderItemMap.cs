using Microsoft.EntityFrameworkCore.Metadata.Builders;
using XCloud.Sales.Data.Domain.Orders;

namespace XCloud.Sales.Data.Mapping.Orders;

public class OrderItemMap : SalesEntityTypeConfiguration<OrderItem>
{
    public override void Configure(EntityTypeBuilder<OrderItem> builder)
    {
        builder.ToTable(nameof(OrderItem));

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Quantity);
        builder.Property(x => x.GoodsName).HasMaxLength(500);
        builder.Property(x => x.UnitPrice).HasPrecision(18, 2);
        builder.Property(o => o.GradePriceOffset).HasPrecision(18, 2);
        builder.Property(x => x.Price).HasPrecision(18, 2);
        builder.Property(x => x.ItemWeight).HasPrecision(18, 2);

        builder.Property(x => x.OrderId).IsRequired().HasMaxLength(100);
        builder.Property(x => x.GoodsId);
        builder.Property(x => x.GoodsSpecCombinationId);

        base.Configure(builder);
    }
}