using Microsoft.EntityFrameworkCore.Metadata.Builders;
using XCloud.Sales.Data.Domain.Localization;

namespace XCloud.Sales.Data.Mapping.Localization;

public class LocaleStringResourceMap : SalesEntityTypeConfiguration<LocaleStringResource>
{
    public override void Configure(EntityTypeBuilder<LocaleStringResource> builder)
    {
        builder.ToTable(nameof(LocaleStringResource));

        builder.HasKey(lsr => lsr.Id);
        builder.Property(lsr => lsr.Name).IsRequired().HasMaxLength(200);
        builder.Property(lsr => lsr.Value).IsRequired();

        base.Configure(builder);

    }
}