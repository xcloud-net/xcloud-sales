using Microsoft.EntityFrameworkCore.Metadata.Builders;
using XCloud.Sales.Data.Domain.Configuration;

namespace XCloud.Sales.Data.Mapping.Configuration;

public class SettingMap : SalesEntityTypeConfiguration<Setting>
{
    public override void Configure(EntityTypeBuilder<Setting> builder)
    {
        builder.ToTable(nameof(Setting));
        builder.HasKey(s => s.Id);
        builder.Property(s => s.Name).IsRequired().HasMaxLength(500);
        builder.Property(s => s.Value).IsRequired();
        base.Configure(builder);
    }
}