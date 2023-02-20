using Microsoft.EntityFrameworkCore.Metadata.Builders;
using XCloud.Sales.Data.Domain.Stores;

namespace XCloud.Sales.Data.Mapping.Stores;

public class StoreMap : SalesEntityTypeConfiguration<Store>
{
    public override void Configure(EntityTypeBuilder<Store> builder)
    {
        builder.ToTable(nameof(Store));

        builder.HasKey(m => m.Id);

        builder.Property(m => m.StoreName).IsRequired().HasMaxLength(100);
        builder.Property(m => m.StoreUrl).HasMaxLength(1000);
        builder.Property(m => m.StoreLogo).HasMaxLength(1000);
        builder.Property(m => m.CopyrightInfo).HasMaxLength(1000);
        builder.Property(m => m.ICPRecord).HasMaxLength(1000);
        builder.Property(m => m.StoreServiceTime).HasMaxLength(1000);
        builder.Property(m => m.ServiceTelePhone).HasMaxLength(1000);
        builder.Property(x => x.StoreClosed);
        builder.Property(x => x.IsActive);
        builder.Property(x => x.CreationTime);
        builder.Property(x => x.IsDeleted);

    }
}