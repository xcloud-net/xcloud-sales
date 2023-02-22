using Microsoft.EntityFrameworkCore.Metadata.Builders;
using XCloud.Database.EntityFrameworkCore.Mapping;
using XCloud.Platform.Core.Domain.Security;

namespace XCloud.Platform.Data.EntityFrameworkCore.Mapping.Security;

public class SysPermissionMap : EfEntityTypeConfiguration<SysPermission>
{
    public override void Configure(EntityTypeBuilder<SysPermission> builder)
    {
        base.Configure(builder);

        builder.ToTable("sys_permission");

        builder.Property(x => x.PermissionKey).IsRequired().HasMaxLength(100);
        builder.Property(x => x.Description).HasMaxLength(500);
        builder.Property(x => x.Group).HasMaxLength(100);
        builder.Property(x => x.AppKey).HasMaxLength(100);
        builder.Property(x => x.CreationTime);
    }
}