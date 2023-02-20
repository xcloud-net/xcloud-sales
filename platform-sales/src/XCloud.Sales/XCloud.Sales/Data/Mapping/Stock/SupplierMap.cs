using Microsoft.EntityFrameworkCore.Metadata.Builders;
using XCloud.Sales.Data.Domain.Stock;

namespace XCloud.Sales.Data.Mapping.Stock;

public class SupplierMap : SalesEntityTypeConfiguration<Supplier>
{
    public override void Configure(EntityTypeBuilder<Supplier> builder)
    {
        builder.ToTable(nameof(Supplier));

        builder.Property(x => x.Name).IsRequired().HasMaxLength(100);
        builder.Property(x => x.ContactName).HasMaxLength(100);
        builder.Property(x => x.Telephone).HasMaxLength(50);
        builder.Property(x => x.Address).HasMaxLength(500);

        builder.Property(x => x.CreationTime);
        builder.Property(x => x.IsDeleted);

        base.Configure(builder);
    }
}