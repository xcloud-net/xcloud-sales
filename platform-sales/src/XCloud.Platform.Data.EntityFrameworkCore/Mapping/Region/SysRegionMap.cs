using Microsoft.EntityFrameworkCore.Metadata.Builders;
using XCloud.Database.EntityFrameworkCore.Mapping;
using XCloud.Platform.Core.Domain.Region;

namespace XCloud.Platform.Data.EntityFrameworkCore.Mapping.Region;

public class SysRegionMap : EfEntityTypeConfiguration<SysRegion>
{
    public override void Configure(EntityTypeBuilder<SysRegion> builder)
    {
        builder.ToTable("sys_region");

        builder.Property(x => x.Name).IsRequired().HasMaxLength(100);
        builder.Property(x => x.Code).IsRequired().HasMaxLength(100);
        builder.Property(x => x.RegionType).HasMaxLength(100);

        builder.Property(x => x.Data);
            
        base.Configure(builder);
    }
}