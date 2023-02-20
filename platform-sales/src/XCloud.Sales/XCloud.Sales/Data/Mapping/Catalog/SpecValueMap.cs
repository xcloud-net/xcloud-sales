using Microsoft.EntityFrameworkCore.Metadata.Builders;
using XCloud.Sales.Data.Domain.Catalog;

namespace XCloud.Sales.Data.Mapping.Catalog;

public class SpecValueMap : SalesEntityTypeConfiguration<SpecValue>
{
    public override void Configure(EntityTypeBuilder<SpecValue> builder)
    {
        builder.ToTable(nameof(SpecValue));

        builder.HasKey(pvav => pvav.Id);

        builder.Property(x => x.GoodsSpecId);
        builder.Property(pvav => pvav.Name).IsRequired().HasMaxLength(400);
        builder.Property(x => x.PriceOffset).HasPrecision(18, 2);

        base.Configure(builder);
    }
}