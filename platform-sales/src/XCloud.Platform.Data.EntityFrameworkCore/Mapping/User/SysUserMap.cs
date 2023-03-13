using Microsoft.EntityFrameworkCore.Metadata.Builders;
using XCloud.Database.EntityFrameworkCore.Mapping;
using XCloud.Platform.Core.Domain.User;
using XCloud.Platform.Shared.Extension;

namespace XCloud.Platform.Data.EntityFrameworkCore.Mapping.User;

public class SysUserMap : EfEntityTypeConfiguration<SysUser>
{
    public override void Configure(EntityTypeBuilder<SysUser> builder)
    {
        builder.ToTable("sys_user");
        builder.MappingAccount();
            
        base.Configure(builder);
    }
}