using Microsoft.EntityFrameworkCore.Metadata.Builders;
using XCloud.Sales.Data.Domain.Catalog;

namespace XCloud.Sales.Data.Mapping.Catalog;

public class GoodsCollectionMap : SalesEntityTypeConfiguration<GoodsCollection>
{
    public override void Configure(EntityTypeBuilder<GoodsCollection> builder)
    {
        builder.ToTable(nameof(GoodsCollection));

        builder.HasKey(x => x.Id);
        builder.Property(x => x.Name).IsRequired().HasMaxLength(100);
        builder.Property(x => x.Description).HasMaxLength(500);
        builder.Property(x => x.Keywords).HasMaxLength(100);
        builder.Property(x => x.ApplyedCount);

        base.Configure(builder);
    }
}