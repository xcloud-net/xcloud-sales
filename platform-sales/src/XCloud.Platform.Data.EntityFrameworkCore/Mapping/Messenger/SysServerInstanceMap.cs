using Microsoft.EntityFrameworkCore.Metadata.Builders;
using XCloud.Database.EntityFrameworkCore.Mapping;
using XCloud.Platform.Core.Domain.Messenger;

namespace XCloud.Platform.Data.EntityFrameworkCore.Mapping.Messenger;

public class SysServerInstanceMap : EfEntityTypeConfiguration<SysServerInstance>
{
    public override void Configure(EntityTypeBuilder<SysServerInstance> builder)
    {
        base.Configure(builder);

        builder.ToTable("sys_server_instance");

        builder.Property(x => x.InstanceId).IsRequired().HasMaxLength(100);
        builder.Property(x => x.ConnectionCount);
        builder.Property(x => x.PingTime);
    }
}