using Microsoft.EntityFrameworkCore.Metadata.Builders;
using XCloud.Database.EntityFrameworkCore.Mapping;
using XCloud.Platform.Core.Domain.Notification;

namespace XCloud.Platform.Data.EntityFrameworkCore.Mapping.Notification;

public class SysNotificationMap : EfEntityTypeConfiguration<SysNotification>
{
    public override void Configure(EntityTypeBuilder<SysNotification> builder)
    {
        builder.ToTable("sys_notification");

        builder.Property(x => x.Title).HasMaxLength(100);
        builder.Property(x => x.Content).HasMaxLength(1000);

        builder.Property(x => x.Read);
        builder.Property(x => x.ReadTime);
        builder.Property(x => x.ActionType).HasMaxLength(100);
        builder.Property(x => x.Data).HasMaxLength(2000);

        builder.Property(x => x.BusinessId).IsRequired().HasMaxLength(100);
        builder.Property(x => x.App).IsRequired().HasMaxLength(100);
        builder.Property(x => x.SenderId).HasMaxLength(100);
        builder.Property(x => x.SenderType).HasMaxLength(50);

        builder.Property(x => x.LastModificationTime);
        builder.Property(x => x.CreationTime);

        base.Configure(builder);
    }
}