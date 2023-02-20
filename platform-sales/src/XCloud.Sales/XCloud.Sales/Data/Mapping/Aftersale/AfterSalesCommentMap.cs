using Microsoft.EntityFrameworkCore.Metadata.Builders;
using XCloud.Sales.Data.Domain.Aftersale;

namespace XCloud.Sales.Data.Mapping.Aftersale;

public class AfterSalesCommentMap : SalesEntityTypeConfiguration<AfterSalesComment>
{
    public override void Configure(EntityTypeBuilder<AfterSalesComment> builder)
    {
        base.Configure(builder);

        builder.ToTable(nameof(AfterSalesComment));

        builder.Property(x => x.AfterSaleId).IsRequired().HasMaxLength(100);
        builder.Property(x => x.Content).IsRequired().HasMaxLength(1000);
        builder.Property(x => x.PictureJson);
        builder.Property(x => x.IsAdmin);

        builder.Property(x => x.CreationTime);
    }
}