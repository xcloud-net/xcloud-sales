using Microsoft.EntityFrameworkCore.Metadata.Builders;
using XCloud.Sales.Data.Domain.WarehouseStock;

namespace XCloud.Sales.Data.Mapping.WarehouseStock;

public class StockUsageHistoryMap : SalesEntityTypeConfiguration<StockUsageHistory>
{
    public override void Configure(EntityTypeBuilder<StockUsageHistory> builder)
    {
        builder.ToTable(nameof(StockUsageHistory));

        builder.Property(x => x.OrderItemId).IsRequired().HasMaxLength(100);
        builder.Property(x => x.WarehouseStockItemId).IsRequired().HasMaxLength(100);
        builder.Property(x => x.Quantity);
        builder.Property(x => x.Revert).HasDefaultValue(false);
        builder.Property(x => x.RevertTime);
        builder.Property(x => x.CreationTime);
    }
}