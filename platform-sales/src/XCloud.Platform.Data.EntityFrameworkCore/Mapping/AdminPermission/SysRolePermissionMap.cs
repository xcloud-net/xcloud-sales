using Microsoft.EntityFrameworkCore.Metadata.Builders;
using XCloud.Database.EntityFrameworkCore.Mapping;
using XCloud.Platform.Core.Domain.AdminPermission;

namespace XCloud.Platform.Data.EntityFrameworkCore.Mapping.AdminPermission;

public class SysRolePermissionMap : EfEntityTypeConfiguration<SysRolePermission>
{
    public override void Configure(EntityTypeBuilder<SysRolePermission> builder)
    {
        builder.ToTable("sys_role_permission");

        builder.Property(x => x.RoleId).IsRequired().HasMaxLength(100);
        builder.Property(x => x.PermissionKey).IsRequired().HasMaxLength(100);

        base.Configure(builder);
    }
}