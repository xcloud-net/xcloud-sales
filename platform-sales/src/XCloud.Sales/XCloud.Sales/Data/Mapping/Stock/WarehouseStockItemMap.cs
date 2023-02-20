using Microsoft.EntityFrameworkCore.Metadata.Builders;
using XCloud.Sales.Data.Domain.Stock;

namespace XCloud.Sales.Data.Mapping.Stock;

public class WarehouseStockItemMap : SalesEntityTypeConfiguration<WarehouseStockItem>
{
    public override void Configure(EntityTypeBuilder<WarehouseStockItem> builder)
    {
        base.Configure(builder);
        builder.ToTable(nameof(WarehouseStockItem));

        builder.HasKey(x => x.Id);

        builder.Property(x => x.WarehouseStockId).IsRequired().HasMaxLength(100);
        builder.Property(x => x.GoodsId);
        builder.Property(x => x.CombinationId);
        builder.Property(x => x.Quantity);
        builder.Property(x => x.Price).HasPrecision(18, 2);

        builder.Property(x => x.DeductQuantity);
        builder.Property(x => x.RuningOut);
    }
}