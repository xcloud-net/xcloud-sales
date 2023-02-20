using Microsoft.EntityFrameworkCore.Metadata.Builders;
using XCloud.Sales.Data.Domain.Logging;

namespace XCloud.Sales.Data.Mapping.Logging;

public class ActivityLogMap : SalesEntityTypeConfiguration<ActivityLog>
{
    public override void Configure(EntityTypeBuilder<ActivityLog> builder)
    {
        builder.ToTable(nameof(ActivityLog));
        builder.HasKey(al => al.Id);

        builder.Property(al => al.ActivityLogTypeId);
        builder.Property(al => al.UserId);
        builder.Property(al => al.AdministratorId).HasMaxLength(100);
        builder.Property(al => al.Comment).HasMaxLength(1000);
        builder.Property(al => al.Value).HasMaxLength(100);
        builder.Property(al => al.Data);
        builder.Property(al => al.UrlReferrer).HasMaxLength(1000);
        builder.Property(al => al.BrowserType).HasMaxLength(100);
        builder.Property(al => al.Device).HasMaxLength(100);
        builder.Property(al => al.UserAgent).HasMaxLength(1000);
        builder.Property(al => al.IpAddress).HasMaxLength(100);
        builder.Property(al => al.RequestPath).HasMaxLength(1000);
        builder.Property(al => al.SubjectType).HasMaxLength(100);
        builder.Property(al => al.SubjectId).HasMaxLength(100);
        builder.Property(al => al.SubjectIntId);

        builder.Property(x => x.GeoCountry).HasMaxLength(200);
        builder.Property(x => x.GeoCity).HasMaxLength(200);
        builder.Property(x => x.Lat);
        builder.Property(x => x.Lng);

        base.Configure(builder);
    }
}