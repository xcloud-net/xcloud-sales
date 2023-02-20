using Microsoft.EntityFrameworkCore.Metadata.Builders;
using XCloud.Sales.Data.Domain.Catalog;

namespace XCloud.Sales.Data.Mapping.Catalog;

public class SpecMap : SalesEntityTypeConfiguration<Spec>
{
    public override void Configure(EntityTypeBuilder<Spec> builder)
    {
        builder.ToTable(nameof(Spec));

        builder.HasKey(pa => pa.Id);

        builder.Property(pa => pa.Name).IsRequired().HasMaxLength(100);
        builder.Property(x => x.GoodsId);

        base.Configure(builder);
    }
}