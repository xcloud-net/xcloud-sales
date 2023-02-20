using Microsoft.EntityFrameworkCore.Metadata.Builders;
using XCloud.Database.EntityFrameworkCore.Mapping;
using XCloud.Platform.Core.Domain.App;

namespace XCloud.Platform.Data.EntityFrameworkCore.Mapping.App;

public class SysAppMap : EfEntityTypeConfiguration<SysApp>
{
    public override void Configure(EntityTypeBuilder<SysApp> builder)
    {
        builder.ToTable("sys_app");

        builder.Property(x => x.Name).IsRequired().HasMaxLength(100);
        builder.Property(x => x.AppKey).IsRequired().HasMaxLength(100);
        builder.Property(x => x.Description).HasMaxLength(1000);
        builder.Property(x => x.Enabled);

        builder.HasIndex(x => x.AppKey).IsUnique();

        base.Configure(builder);
    }
}