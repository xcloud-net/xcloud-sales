using Microsoft.EntityFrameworkCore.Metadata.Builders;
using XCloud.Sales.Data.Domain.Shipping;

namespace XCloud.Sales.Data.Mapping.Shipping;

public class ShipmentOrderGoodsMap : SalesEntityTypeConfiguration<ShipmentOrderItem>
{
    public override void Configure(EntityTypeBuilder<ShipmentOrderItem> builder)
    {
        builder.ToTable(nameof(ShipmentOrderItem));
        builder.HasKey(sopv => sopv.Id);

        builder.Property(s => s.ShipmentId).IsRequired().HasMaxLength(100);
        builder.Property(s => s.OrderItemId).IsRequired();

        base.Configure(builder);
    }
}