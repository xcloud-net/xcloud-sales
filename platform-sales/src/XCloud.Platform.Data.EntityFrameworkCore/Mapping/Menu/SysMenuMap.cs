using Microsoft.EntityFrameworkCore.Metadata.Builders;
using XCloud.Database.EntityFrameworkCore.Mapping;
using XCloud.Platform.Core.Domain.Menu;

namespace XCloud.Platform.Data.EntityFrameworkCore.Mapping.Menu;

public class SysMenuMap : EfEntityTypeConfiguration<SysMenu>
{
    public override void Configure(EntityTypeBuilder<SysMenu> builder)
    {
        builder.ToTable("sys_menu");

        builder.Property(x => x.Name).IsRequired().HasMaxLength(100);
        builder.Property(x => x.Description).HasMaxLength(1000);

        builder.Property(x => x.IconCls).HasMaxLength(100);
        builder.Property(x => x.IconUrl).HasMaxLength(500);
        builder.Property(x => x.Url).HasMaxLength(1000);
        builder.Property(x => x.ImageUrl).HasMaxLength(500);

        builder.Property(x => x.ParentId).HasMaxLength(100);

        builder.Ignore(x => x.RequiredPermissions);
            
        base.Configure(builder);
    }
}