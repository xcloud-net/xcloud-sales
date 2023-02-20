
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace XCloud.Sales.Data.Mapping.Promotion;

public class StorePromotionMap : SalesEntityTypeConfiguration<Domain.Promotion.StorePromotion>
{
    public override void Configure(EntityTypeBuilder<Domain.Promotion.StorePromotion> builder)
    {
        builder.ToTable(nameof(Domain.Promotion.StorePromotion));
        builder.HasKey(c => c.Id);

        builder.Property(u => u.Name).HasMaxLength(100);
        builder.Property(u => u.Description).HasMaxLength(1000);

        builder.Property(u => u.Condition);
        builder.Property(u => u.Result);
            
        builder.Property(u => u.Order);
        builder.Property(u => u.StartTime);
        builder.Property(u => u.EndTime);
        builder.Property(u => u.IsExclusive);
        builder.Property(u => u.IsDeleted);

        builder.Property(x => x.IsActive);
        builder.Property(x => x.CreationTime);
            
        base.Configure(builder);
    }
}