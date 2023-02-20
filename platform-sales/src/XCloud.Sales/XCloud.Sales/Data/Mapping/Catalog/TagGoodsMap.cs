using Microsoft.EntityFrameworkCore.Metadata.Builders;
using XCloud.Sales.Data.Domain.Catalog;

namespace XCloud.Sales.Data.Mapping.Catalog;

public class TagGoodsMap : SalesEntityTypeConfiguration<TagGoods>
{
    public override void Configure(EntityTypeBuilder<TagGoods> builder)
    {
        builder.ToTable(nameof(TagGoods));

        builder.HasKey(x => x.Id);
        builder.Property(x => x.TagId).IsRequired().HasMaxLength(100);
        builder.Property(x => x.GoodsId).IsRequired();

        base.Configure(builder);
    }
}