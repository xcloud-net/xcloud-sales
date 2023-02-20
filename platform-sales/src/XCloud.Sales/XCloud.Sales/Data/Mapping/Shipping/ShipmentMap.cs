using Microsoft.EntityFrameworkCore.Metadata.Builders;
using XCloud.Sales.Data.Domain.Shipping;

namespace XCloud.Sales.Data.Mapping.Shipping;

public class ShipmentMap : SalesEntityTypeConfiguration<Shipment>
{
    public override void Configure(EntityTypeBuilder<Shipment> builder)
    {
        builder.ToTable(nameof(Shipment));
        builder.HasKey(s => s.Id);

        builder.Property(s => s.OrderId).IsRequired().HasMaxLength(100);
        builder.Property(s => s.ExpressName).HasMaxLength(100);
        builder.Property(s => s.TrackingNumber).HasMaxLength(100);
        builder.Property(s => s.TotalWeight).HasPrecision(18, 2);

        base.Configure(builder);
    }
}