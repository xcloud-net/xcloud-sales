using Microsoft.EntityFrameworkCore.Metadata.Builders;
using XCloud.Sales.Data.Domain.Media;

namespace XCloud.Sales.Data.Mapping.Media;

public class PictureMap : SalesEntityTypeConfiguration<Picture>
{
    public override void Configure(EntityTypeBuilder<Picture> builder)
    {
        builder.ToTable(nameof(Picture));
        builder.HasKey(p => p.Id);
        builder.Property(p => p.MimeType).IsRequired().HasMaxLength(40);
        builder.Property(p => p.SeoFilename).HasMaxLength(300);
        builder.Property(x => x.ResourceId).HasMaxLength(100);
        builder.Property(x => x.ResourceData);

        base.Configure(builder);
    }
}