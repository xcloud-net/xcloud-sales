using Microsoft.EntityFrameworkCore.Metadata.Builders;
using XCloud.Sales.Data.Domain.Catalog;

namespace XCloud.Sales.Data.Mapping.Catalog;

public class GoodsReviewMap : SalesEntityTypeConfiguration<GoodsReview>
{
    public override void Configure(EntityTypeBuilder<GoodsReview> builder)
    {
        builder.ToTable(nameof(GoodsReview));
        builder.HasKey(o => o.Id);
        builder.Property(pr => pr.Title).HasMaxLength(500);
        builder.Property(pr => pr.ReviewText).HasMaxLength(1000);

        builder.Property(x => x.GoodsId);
        builder.Property(x => x.UserId);
        builder.Property(x => x.OrderId).IsRequired().HasMaxLength(100);
        builder.Property(x => x.Rating);
        builder.Property(x => x.IpAddress).HasMaxLength(100);
        builder.Property(x => x.CreationTime);

        base.Configure(builder);
    }
}