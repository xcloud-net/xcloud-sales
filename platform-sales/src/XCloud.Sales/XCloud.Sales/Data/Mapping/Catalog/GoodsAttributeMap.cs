using Microsoft.EntityFrameworkCore.Metadata.Builders;
using XCloud.Sales.Data.Domain.Catalog;

namespace XCloud.Sales.Data.Mapping.Catalog;

public class GoodsAttributeMap : SalesEntityTypeConfiguration<GoodsAttribute>
{
    public override void Configure(EntityTypeBuilder<GoodsAttribute> builder)
    {
        base.Configure(builder);
        builder.ToTable(nameof(GoodsAttribute));

        builder.Property(x => x.GoodsId);
        builder.Property(x => x.Name).IsRequired().HasMaxLength(100);
        builder.Property(x => x.Value).IsRequired().HasMaxLength(100);
        builder.Property(x => x.CreationTime);
    }
}