using Microsoft.EntityFrameworkCore.Metadata.Builders;
using XCloud.Sales.Data.Domain.Users;

namespace XCloud.Sales.Data.Mapping.Users;

public class BalanceHistoryMap : SalesEntityTypeConfiguration<BalanceHistory>
{
    public override void Configure(EntityTypeBuilder<BalanceHistory> builder)
    {
        builder.ToTable(nameof(BalanceHistory));
        builder.HasKey(rph => rph.Id);

        builder.Property(x => x.UserId);
        builder.Property(x => x.Balance).HasPrecision(18, 2);
        builder.Property(x => x.LatestBalance).HasPrecision(18, 2);
        builder.Property(x => x.ActionType);
        builder.Property(x => x.Message).HasMaxLength(300);

        base.Configure(builder);
    }
}