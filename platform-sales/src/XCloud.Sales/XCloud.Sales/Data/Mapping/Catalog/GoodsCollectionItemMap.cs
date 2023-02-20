using Microsoft.EntityFrameworkCore.Metadata.Builders;
using XCloud.Sales.Data.Domain.Catalog;

namespace XCloud.Sales.Data.Mapping.Catalog;

public class GoodsCollectionItemMap : SalesEntityTypeConfiguration<GoodsCollectionItem>
{
    public override void Configure(EntityTypeBuilder<GoodsCollectionItem> builder)
    {
        builder.ToTable(nameof(GoodsCollectionItem));

        builder.HasKey(x => x.Id);
        builder.Property(x => x.CollectionId).IsRequired().HasMaxLength(100);
        builder.Property(x => x.GoodsId);
        builder.Property(x => x.GoodsSpecCombinationId);
        builder.Property(x => x.Quantity);

        base.Configure(builder);
    }
}