using Microsoft.EntityFrameworkCore.Metadata.Builders;
using XCloud.Sales.Data.Domain.Coupons;

namespace XCloud.Sales.Data.Mapping.Coupons;

public class CouponUserMap : SalesEntityTypeConfiguration<CouponUserMapping>
{
    public override void Configure(EntityTypeBuilder<CouponUserMapping> builder)
    {
        builder.ToTable(nameof(CouponUserMapping));

        builder.HasKey(p => p.Id);

        builder.Property(x => x.UserId);
        builder.Property(x => x.CouponId);
        builder.Property(x => x.IsUsed);
        builder.Property(x => x.UsedTime);
        builder.Property(x => x.ExpiredAt);
        builder.Property(x => x.Value).HasPrecision(18, 2);
        builder.Property(x => x.MinimumConsumption).HasPrecision(18, 2);

        builder.Property(x => x.CreationTime);

        base.Configure(builder);
    }
}