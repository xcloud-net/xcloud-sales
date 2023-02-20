using Microsoft.EntityFrameworkCore.Metadata.Builders;
using XCloud.Sales.Data.Domain.Shipping;

namespace XCloud.Sales.Data.Mapping.Shipping;

public class ShippingMethodMap : SalesEntityTypeConfiguration<ShippingMethod>
{
    public override void Configure(EntityTypeBuilder<ShippingMethod> builder)
    {
        builder.ToTable(nameof(ShippingMethod));
        builder.HasKey(sm => sm.Id);
        builder.Property(sm => sm.Name).IsRequired().HasMaxLength(400);
        builder.Property(x => x.Description).HasMaxLength(500);

        base.Configure(builder);
    }
}