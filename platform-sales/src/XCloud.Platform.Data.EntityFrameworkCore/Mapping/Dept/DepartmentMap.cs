using Microsoft.EntityFrameworkCore.Metadata.Builders;
using XCloud.Database.EntityFrameworkCore.Mapping;
using XCloud.Platform.Core.Domain.Dept;

namespace XCloud.Platform.Data.EntityFrameworkCore.Mapping.Dept;

public class DepartmentMap : EfEntityTypeConfiguration<Department>
{
    public override void Configure(EntityTypeBuilder<Department> builder)
    {
        builder.ToTable("sys_department");
        builder.Property(x => x.Name).IsRequired().HasMaxLength(100);
        builder.Property(x => x.Description).HasMaxLength(1000);
        builder.Property(x => x.LogoUrl).HasMaxLength(1000);
            
        base.Configure(builder);
    }
}