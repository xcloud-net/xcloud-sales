using Microsoft.EntityFrameworkCore.Metadata.Builders;
using XCloud.Sales.Data.Domain.Users;

namespace XCloud.Sales.Data.Mapping.Users;

public class PointsHistoryMap : SalesEntityTypeConfiguration<PointsHistory>
{
    public override void Configure(EntityTypeBuilder<PointsHistory> builder)
    {
        builder.ToTable(nameof(PointsHistory));
        builder.HasKey(rph => rph.Id);

        builder.Property(x => x.UserId);
        builder.Property(x => x.OrderId).HasMaxLength(100);
        builder.Property(x => x.Points);
        builder.Property(x => x.PointsBalance);
        builder.Property(x => x.ActionType);
        builder.Property(x => x.Message).HasMaxLength(300);
        builder.Property(rph => rph.UsedAmount).HasPrecision(18, 2);

        base.Configure(builder);
    }
}