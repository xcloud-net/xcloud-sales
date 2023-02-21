using Microsoft.EntityFrameworkCore.Metadata.Builders;
using XCloud.Sales.Data.Domain.WarehouseStock;

namespace XCloud.Sales.Data.Mapping.WarehouseStock;

public class StockItemMap : SalesEntityTypeConfiguration<StockItem>
{
    public override void Configure(EntityTypeBuilder<StockItem> builder)
    {
        base.Configure(builder);
        builder.ToTable(nameof(StockItem));

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