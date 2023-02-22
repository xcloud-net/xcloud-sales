using Microsoft.EntityFrameworkCore.Metadata.Builders;
using XCloud.Database.EntityFrameworkCore.Mapping;
using XCloud.Platform.Core.Domain.Security;

namespace XCloud.Platform.Data.EntityFrameworkCore.Mapping.Security;

public class SysRoleMap : EfEntityTypeConfiguration<SysRole>
{
    public override void Configure(EntityTypeBuilder<SysRole> builder)
    {
        builder.ToTable("sys_role");

        builder.Property(x => x.Name).IsRequired().HasMaxLength(200);

        builder.Property(x => x.Description).HasMaxLength(100);

        base.Configure(builder);
    }
}