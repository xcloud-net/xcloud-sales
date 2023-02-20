using Microsoft.EntityFrameworkCore.Metadata.Builders;
using XCloud.Sales.Data.Domain.Coupons;

namespace XCloud.Sales.Data.Mapping.Coupons;

public class CouponMap : SalesEntityTypeConfiguration<Coupon>
{
    public override void Configure(EntityTypeBuilder<Coupon> builder)
    {
        builder.ToTable(nameof(Coupon));
        builder.HasKey(p => p.Id);

        builder.Property(p => p.Title).IsRequired().HasMaxLength(500);
        builder.Property(p => p.MinimumConsumption).HasPrecision(18, 2);
        builder.Property(p => p.Value).HasPrecision(18, 2);
        builder.Property(x => x.StartTime);
        builder.Property(x => x.EndTime);
        builder.Property(x => x.ExpiredDaysFromIssue);
            
        builder.Property(x => x.Amount);
        builder.Property(x => x.IsAmountLimit);
        builder.Property(x => x.AccountIssuedLimitCount);
            
        builder.Property(x => x.UsedAmount);
        builder.Property(x => x.IssuedAmount);

        builder.Property(x => x.CreationTime);
        builder.Property(x => x.IsDeleted);

        base.Configure(builder);

    }
}