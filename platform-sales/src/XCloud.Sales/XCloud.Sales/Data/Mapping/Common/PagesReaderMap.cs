
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using XCloud.Sales.Data.Domain.Common;

namespace XCloud.Sales.Data.Mapping.Common;

public class PagesReaderMap : SalesEntityTypeConfiguration<PagesReader>
{
    public override void Configure(EntityTypeBuilder<PagesReader> builder)
    {
        builder.ToTable(nameof(PagesReader));

        builder.HasKey(c => c.Id);
        builder.Property(x => x.PageId).IsRequired().HasMaxLength(100);
        builder.Property(u => u.UserId);

        base.Configure(builder);
    }
}