using Microsoft.EntityFrameworkCore.Metadata.Builders;
using XCloud.Database.EntityFrameworkCore.Mapping;
using XCloud.Platform.Core.Domain.Security;

namespace XCloud.Platform.Data.EntityFrameworkCore.Mapping.Security;

public class SysAdminRoleMap : EfEntityTypeConfiguration<SysAdminRole>
{
    public override void Configure(EntityTypeBuilder<SysAdminRole> builder)
    {
        builder.ToTable("sys_admin_role");

        builder.Property(x => x.RoleId).IsRequired().HasMaxLength(100);
        builder.Property(x => x.AdminId).IsRequired().HasMaxLength(100);

        base.Configure(builder);
    }
}