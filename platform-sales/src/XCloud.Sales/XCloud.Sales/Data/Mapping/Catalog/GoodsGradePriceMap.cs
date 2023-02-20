using Microsoft.EntityFrameworkCore.Metadata.Builders;
using XCloud.Sales.Data.Domain.Catalog;

namespace XCloud.Sales.Data.Mapping.Catalog;

public class GoodsGradePriceMap : SalesEntityTypeConfiguration<GoodsGradePrice>
{
    public override void Configure(EntityTypeBuilder<GoodsGradePrice> builder)
    {
        builder.ToTable(nameof(GoodsGradePrice));

        builder.Property(x => x.GoodsId);
        builder.Property(x => x.GoodsCombinationId);
        builder.Property(x => x.GradeId).IsRequired().HasMaxLength(100);
        builder.Property(x => x.Price).HasPrecision(18, 2);

        base.Configure(builder);
    }
}