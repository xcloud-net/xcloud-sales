using Microsoft.EntityFrameworkCore.Metadata.Builders;
using XCloud.Sales.Data.Domain.Stores;

namespace XCloud.Sales.Data.Mapping.Stores;

public class StoreManagerMap : SalesEntityTypeConfiguration<StoreManager>
{
    public override void Configure(EntityTypeBuilder<StoreManager> builder)
    {
        builder.ToTable(nameof(StoreManager));
        builder.HasKey(m => m.Id);
        builder.Property(m => m.StoreId).IsRequired().HasMaxLength(100);
        builder.Property(m => m.GlobalUserId).IsRequired().HasMaxLength(100);
        builder.Property(x => x.NickName).HasMaxLength(100);
        builder.Property(x => x.Avatar).HasMaxLength(1000);
        builder.Property(x => x.Role);
        builder.Property(x => x.IsActive);
        builder.Property(x => x.CreationTime);
        builder.Property(x => x.IsDeleted);
    }
}