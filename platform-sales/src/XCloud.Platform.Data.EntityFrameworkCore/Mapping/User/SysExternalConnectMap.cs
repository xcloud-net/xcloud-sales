using Microsoft.EntityFrameworkCore.Metadata.Builders;
using XCloud.Database.EntityFrameworkCore.Mapping;
using XCloud.Platform.Core.Domain.User;

namespace XCloud.Platform.Data.EntityFrameworkCore.Mapping.User;

public class SysExternalConnectMap : EfEntityTypeConfiguration<SysExternalConnect>
{
    public override void Configure(EntityTypeBuilder<SysExternalConnect> builder)
    {
        builder.ToTable("sys_external_connect");

        builder.Property(x => x.UserId).IsRequired().HasMaxLength(100);

        builder.Property(x => x.Platform).IsRequired().HasMaxLength(100);
        builder.Property(x => x.AppId).IsRequired().HasMaxLength(100);
        builder.Property(x => x.OpenId).IsRequired().HasMaxLength(100);

        base.Configure(builder);
    }
}