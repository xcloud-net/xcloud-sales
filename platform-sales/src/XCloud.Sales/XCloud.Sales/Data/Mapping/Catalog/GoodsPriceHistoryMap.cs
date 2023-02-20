using Microsoft.EntityFrameworkCore.Metadata.Builders;
using XCloud.Sales.Data.Domain.Catalog;

namespace XCloud.Sales.Data.Mapping.Catalog;

public class GoodsPriceHistoryMap : SalesEntityTypeConfiguration<GoodsPriceHistory>
{
    public override void Configure(EntityTypeBuilder<GoodsPriceHistory> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(x => x.GoodsId).IsRequired();
        builder.Property(x => x.GoodsSpecCombinationId).IsRequired();

        builder.Property(x => x.PreviousPrice).HasPrecision(18, 2);
        builder.Property(x => x.Price).HasPrecision(18, 2);
        builder.Property(x => x.Comment).HasMaxLength(500);

        base.Configure(builder);
    }
}