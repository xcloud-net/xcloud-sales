using Microsoft.EntityFrameworkCore.Metadata.Builders;
using XCloud.Sales.Data.Domain.Users;

namespace XCloud.Sales.Data.Mapping.Users;

public class PrepaidCardMap : SalesEntityTypeConfiguration<PrepaidCard>
{
    public override void Configure(EntityTypeBuilder<PrepaidCard> builder)
    {
        base.Configure(builder);

        builder.ToTable(nameof(PrepaidCard));

        builder.Property(x => x.Amount).HasPrecision(18, 2);
        builder.Property(x => x.EndTime);

        builder.Property(x => x.Used);
        builder.Property(x => x.UserId);
        builder.Property(x => x.UsedTime);

        builder.Property(x => x.IsActive);
    }
}