using Microsoft.EntityFrameworkCore.Metadata.Builders;
using XCloud.Database.EntityFrameworkCore.Mapping;
using XCloud.Platform.Core.Domain.Settings;

namespace XCloud.Platform.Data.EntityFrameworkCore.Mapping.Settings;

public class SysSettingsMap : EfEntityTypeConfiguration<SysSettings>
{
    public override void Configure(EntityTypeBuilder<SysSettings> builder)
    {
        builder.ToTable("sys_settings");

        builder.Property(x => x.Name).IsRequired().HasMaxLength(300);
        builder.Property(x => x.Value);
            
        base.Configure(builder);
    }
}