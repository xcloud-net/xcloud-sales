using Microsoft.EntityFrameworkCore.Metadata.Builders;
using XCloud.Sales.Data.Domain.Catalog;

namespace XCloud.Sales.Data.Mapping.Catalog;

public class GoodsSpecCombinationMap : SalesEntityTypeConfiguration<GoodsSpecCombination>
{
    public override void Configure(EntityTypeBuilder<GoodsSpecCombination> builder)
    {
        builder.ToTable(nameof(GoodsSpecCombination));

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Name).IsRequired().HasMaxLength(100);
        builder.Property(x => x.Description).HasMaxLength(500);
        builder.Property(x => x.Sku).HasMaxLength(100);
        builder.Property(x => x.Price).HasPrecision(18, 2);
        builder.Property(x => x.CostPrice).HasPrecision(18, 2);
        builder.Property(x => x.Color).HasMaxLength(100);
        builder.Property(x => x.Weight);
        builder.Property(x => x.StockQuantity);
        builder.Property(x => x.PictureId);
        builder.Property(x => x.GoodsId);
        builder.Property(x => x.SpecificationsJson).HasComment("规格配置参数");
        builder.Property(x => x.IsActive);

        base.Configure(builder);
    }
}