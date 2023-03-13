using Microsoft.EntityFrameworkCore.Metadata.Builders;
using XCloud.Database.EntityFrameworkCore.Mapping;
using XCloud.Platform.Core.Domain.Admin;
using XCloud.Platform.Shared.Extension;

namespace XCloud.Platform.Data.EntityFrameworkCore.Mapping.Admin;

public class SysAdminMap : EfEntityTypeConfiguration<SysAdmin>
{
    public override void Configure(EntityTypeBuilder<SysAdmin> builder)
    {
        builder.ToTable("sys_admin");
        
        builder.MappingHasUserId(isRequired: false);

        builder.Property(x => x.IsActive);
        builder.Property(x => x.IsSuperAdmin);
        
        base.Configure(builder);

    }
}