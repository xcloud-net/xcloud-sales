using Microsoft.EntityFrameworkCore.Metadata.Builders;
using XCloud.Sales.Data.Domain.WarehouseStock;

namespace XCloud.Sales.Data.Mapping.WarehouseStock;

public class WarehouseMap : SalesEntityTypeConfiguration<Warehouse>
{
    public override void Configure(EntityTypeBuilder<Warehouse> builder)
    {
        builder.ToTable(nameof(Warehouse));

        builder.Property(x => x.Name).IsRequired().HasMaxLength(100);
        builder.Property(x => x.Address).HasMaxLength(500);
        builder.Property(x => x.Lng);
        builder.Property(x => x.Lat);

        builder.Property(x => x.CreationTime);
        builder.Property(x => x.IsDeleted);

        base.Configure(builder);
    }
}