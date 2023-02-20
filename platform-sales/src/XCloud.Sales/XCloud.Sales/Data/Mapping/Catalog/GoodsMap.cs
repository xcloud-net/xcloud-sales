using Microsoft.EntityFrameworkCore.Metadata.Builders;
using XCloud.Sales.Data.Domain.Catalog;

namespace XCloud.Sales.Data.Mapping.Catalog;

public class GoodsMap : SalesEntityTypeConfiguration<Goods>
{
    public override void Configure(EntityTypeBuilder<Goods> builder)
    {
        builder.ToTable(nameof(Goods));

        builder.HasKey(p => p.Id);

        builder.Property(p => p.Name).IsRequired().HasMaxLength(400);
        builder.Property(p => p.SeoName).IsRequired().HasMaxLength(400);
        builder.Property(p => p.ShortDescription).HasMaxLength(1000);
        builder.Property(p => p.FullDescription);
        builder.Property(p => p.AdminComment).HasMaxLength(500);

        builder.Property(x => x.AttributeType);
        builder.Property(x => x.Keywords).HasMaxLength(1000);

        builder.Property(x => x.MaxAmountInOnePurchase);

        builder.Property(x => x.IsNew);
        builder.Property(x => x.IsHot);
        builder.Property(x => x.StickyTop);

        builder.Property(x => x.StockQuantity);
        builder.Property(x => x.SaleVolume);
        builder.Property(x => x.CostPrice).HasPrecision(18, 2);
        builder.Property(x => x.Price).HasPrecision(18, 2);
        builder.Property(x => x.MinPrice).HasPrecision(18, 2);
        builder.Property(x => x.MaxPrice).HasPrecision(18, 2);
        builder.Property(x => x.CategoryId);
        builder.Property(x => x.BrandId);
        builder.Property(x => x.ApprovedReviewsRates);
        builder.Property(x => x.ApprovedTotalReviews);
        builder.Property(x => x.Published);
        builder.Property(x => x.IsDeleted);
        builder.Property(x => x.CreationTime);
        builder.Property(x => x.LastModificationTime);

        base.Configure(builder);
    }
}