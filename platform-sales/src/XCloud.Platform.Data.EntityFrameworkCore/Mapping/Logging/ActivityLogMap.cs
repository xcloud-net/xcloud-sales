using Microsoft.EntityFrameworkCore.Metadata.Builders;
using XCloud.Database.EntityFrameworkCore.Mapping;
using XCloud.Platform.Core.Domain.Logging;

namespace XCloud.Platform.Data.EntityFrameworkCore.Mapping.Logging;

public class ActivityLogMap : EfEntityTypeConfiguration<ActivityLog>
{
    public override void Configure(EntityTypeBuilder<ActivityLog> builder)
    {
        builder.ToTable("sys_activity_log");

        builder.Property(x => x.SubjectId).HasMaxLength(100);
        builder.Property(x => x.SubjectType).HasMaxLength(100);
        builder.Property(x => x.LogType).HasMaxLength(100);
        builder.Property(x => x.AppId).HasMaxLength(100);
        builder.Property(x => x.ActionName).HasMaxLength(100);

        builder.Property(x => x.Log);
        builder.Property(x => x.Data);
        builder.Property(x => x.ExceptionDetail);
        builder.Property(x => x.LogTime);

        base.Configure(builder);
    }
}