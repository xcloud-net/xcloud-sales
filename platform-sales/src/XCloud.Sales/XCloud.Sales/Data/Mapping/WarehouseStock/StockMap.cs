using Microsoft.EntityFrameworkCore.Metadata.Builders;
using XCloud.Sales.Data.Domain.WarehouseStock;

namespace XCloud.Sales.Data.Mapping.WarehouseStock;

public class StockMap : SalesEntityTypeConfiguration<Stock>
{
    public override void Configure(EntityTypeBuilder<Stock> builder)
    {
        base.Configure(builder);

        builder.ToTable(nameof(Stock));

        builder.HasKey(x => x.Id);

        builder.Property(x => x.No).IsRequired().HasMaxLength(100);
        builder.Property(x => x.SupplierId).HasMaxLength(100);
        builder.Property(x => x.WarehouseId).HasMaxLength(100);
        builder.Property(x => x.Remark).HasMaxLength(1000);
        builder.Property(x => x.Approved);
        builder.Property(x => x.ApprovedByUserId).HasMaxLength(100);
        builder.Property(x => x.ApprovedTime);
        builder.Property(x => x.ExpirationTime);
    }
}