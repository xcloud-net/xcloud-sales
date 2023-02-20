using Microsoft.EntityFrameworkCore.Metadata.Builders;
using XCloud.Sales.Data.Domain.Catalog;

namespace XCloud.Sales.Data.Mapping.Catalog;

public class BrandMap : SalesEntityTypeConfiguration<Brand>
{
    public override void Configure(EntityTypeBuilder<Brand> builder)
    {
        builder.ToTable(nameof(Brand));

        builder.HasKey(m => m.Id);
        builder.Property(m => m.Name).IsRequired().HasMaxLength(400);
        builder.Property(m => m.Description).HasMaxLength(500);
        builder.Property(m => m.MetaKeywords).HasMaxLength(400);
        builder.Property(m => m.MetaDescription).HasMaxLength(500);
        builder.Property(m => m.MetaTitle).HasMaxLength(400);
        builder.Property(m => m.PriceRanges).HasMaxLength(400);

        base.Configure(builder);
    }
}