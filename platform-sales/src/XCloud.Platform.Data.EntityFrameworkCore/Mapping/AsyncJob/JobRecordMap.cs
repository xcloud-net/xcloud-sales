using Microsoft.EntityFrameworkCore.Metadata.Builders;
using XCloud.Database.EntityFrameworkCore.Mapping;
using XCloud.Platform.Core.Domain.AsyncJob;

namespace XCloud.Platform.Data.EntityFrameworkCore.Mapping.AsyncJob;

public class JobRecordMap : EfEntityTypeConfiguration<JobRecord>
{
    public override void Configure(EntityTypeBuilder<JobRecord> builder)
    {
        builder.ToTable("sys_job");

        builder.Property(x => x.JobKey).IsRequired().HasMaxLength(200);

        builder.Property(x => x.Desc).HasMaxLength(200);
        builder.Property(x => x.ExceptionMessage).HasMaxLength(1000);
        builder.Property(x => x.ExtraData);
        builder.Property(x => x.StartTime);
        builder.Property(x => x.EndTime);
        builder.Property(x => x.Status);
            
        base.Configure(builder);
    }
}