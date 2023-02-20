using Microsoft.EntityFrameworkCore.Metadata.Builders;
using XCloud.Sales.Data.Domain.Common;

namespace XCloud.Sales.Data.Mapping.Common;

public class PagesMap : SalesEntityTypeConfiguration<Pages>
{
    public override void Configure(EntityTypeBuilder<Pages> builder)
    {
        builder.ToTable(nameof(Pages));

        builder.HasKey(c => c.Id);
        builder.Property(x => x.SeoName).IsRequired().HasMaxLength(500);
        builder.Property(u => u.Title).HasMaxLength(100);
        builder.Property(u => u.Description).HasMaxLength(1000);
        builder.Property(x => x.CoverImageResourceData);
        builder.Property(u => u.BodyContent);

        builder.Property(x => x.ReadCount);
        builder.Property(x => x.IsPublished);
        builder.Property(x => x.PublishedTime);

        base.Configure(builder);
    }
}