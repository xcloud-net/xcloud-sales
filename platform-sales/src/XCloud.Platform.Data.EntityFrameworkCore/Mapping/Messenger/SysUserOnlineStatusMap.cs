using Microsoft.EntityFrameworkCore.Metadata.Builders;
using XCloud.Database.EntityFrameworkCore.Mapping;
using XCloud.Platform.Core.Domain.Messenger;

namespace XCloud.Platform.Data.EntityFrameworkCore.Mapping.Messenger;

public class SysUserOnlineStatusMap : EfEntityTypeConfiguration<SysUserOnlineStatus>
{
    public override void Configure(EntityTypeBuilder<SysUserOnlineStatus> builder)
    {
        base.Configure(builder);

        builder.ToTable("sys_user_online_status");

        builder.Property(x => x.UserId).IsRequired().HasMaxLength(100);
        builder.Property(x => x.DeviceId).IsRequired().HasMaxLength(100);
        builder.Property(x => x.ServerInstanceId).IsRequired().HasMaxLength(100);

        builder.Property(x => x.PingTime);
    }
}