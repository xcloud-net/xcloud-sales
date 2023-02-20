using Microsoft.EntityFrameworkCore.Metadata.Builders;
using XCloud.Database.EntityFrameworkCore.Mapping;
using XCloud.Platform.Core.Domain.User;

namespace XCloud.Platform.Data.EntityFrameworkCore.Mapping.User;

public class SysWxUnionMap : EfEntityTypeConfiguration<SysWxUnion>
{
    public override void Configure(EntityTypeBuilder<SysWxUnion> builder)
    {
        builder.ToTable("sys_wx_union");

        builder.Property(x => x.Platform).IsRequired().HasMaxLength(100);
        builder.Property(x => x.AppId).IsRequired().HasMaxLength(100);
        builder.Property(x => x.OpenId).IsRequired().HasMaxLength(100);
        builder.Property(x => x.UnionId).IsRequired().HasMaxLength(100);

        base.Configure(builder);
    }
}