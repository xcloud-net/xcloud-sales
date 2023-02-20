using Microsoft.EntityFrameworkCore.Metadata.Builders;
using XCloud.Sales.Data.Domain.Users;

namespace XCloud.Sales.Data.Mapping.Users;

public class FavoritesMap : SalesEntityTypeConfiguration<Favorites>
{
    public override void Configure(EntityTypeBuilder<Favorites> builder)
    {
        builder.ToTable(nameof(Favorites));
        builder.HasKey(a => a.Id);

        base.Configure(builder);
    }
}