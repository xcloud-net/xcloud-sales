using Microsoft.EntityFrameworkCore.Metadata.Builders;
using XCloud.Sales.Data.Domain.Catalog;

namespace XCloud.Sales.Data.Mapping.Catalog;

public class GoodsPictureMap : SalesEntityTypeConfiguration<GoodsPicture>
{
    public override void Configure(EntityTypeBuilder<GoodsPicture> builder)
    {
        builder.ToTable(nameof(GoodsPicture));
        builder.HasKey(pp => pp.Id);

        builder.Property(x => x.GoodsId);
        builder.Property(x => x.CombinationId);
        builder.Property(x => x.PictureId);
        builder.Property(x => x.DisplayOrder);

        base.Configure(builder);
    }
}