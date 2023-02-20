using Microsoft.EntityFrameworkCore.Metadata.Builders;
using XCloud.Sales.Data.Domain.Users;

namespace XCloud.Sales.Data.Mapping.Users;

public class UserMap : SalesEntityTypeConfiguration<User>
{
    public override void Configure(EntityTypeBuilder<User> builder)
    {
        builder.ToTable(nameof(User));
        builder.HasKey(c => c.Id);
        builder.Property(x => x.GlobalUserId).HasMaxLength(100);
        builder.Property(x => x.NickName).HasMaxLength(100);
        builder.Property(x => x.Avatar).HasMaxLength(1000);
        builder.Property(x => x.AccountMobile).HasMaxLength(100);
        builder.Property(x => x.Balance).HasPrecision(18, 2);
        builder.Property(x => x.Points);
        builder.Property(x => x.HistoryPoints);

        base.Configure(builder);
    }
}