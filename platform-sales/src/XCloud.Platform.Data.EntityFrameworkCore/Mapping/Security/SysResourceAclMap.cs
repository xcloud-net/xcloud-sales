using Microsoft.EntityFrameworkCore.Metadata.Builders;
using XCloud.Database.EntityFrameworkCore.Mapping;
using XCloud.Platform.Core.Domain.Security;

namespace XCloud.Platform.Data.EntityFrameworkCore.Mapping.Security;

public class SysResourceAclMap : EfEntityTypeConfiguration<SysResourceAcl>
{
    public override void Configure(EntityTypeBuilder<SysResourceAcl> builder)
    {
        builder.ToTable("sys_resource_acl");

        //resource
        builder.Property(x => x.ResourceType).IsRequired().HasMaxLength(100);
        builder.Property(x => x.ResourceId).IsRequired().HasMaxLength(100);
        //permission type
        builder.Property(x => x.PermissionType).IsRequired().HasMaxLength(100);
        builder.Property(x => x.PermissionId).IsRequired().HasMaxLength(100);

        builder.Property(x => x.AccessControlType);

        base.Configure(builder);
    }
}