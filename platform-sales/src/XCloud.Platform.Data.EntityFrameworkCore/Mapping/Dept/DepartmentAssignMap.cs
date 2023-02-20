using Microsoft.EntityFrameworkCore.Metadata.Builders;
using XCloud.Database.EntityFrameworkCore.Mapping;
using XCloud.Platform.Core.Domain.Dept;

namespace XCloud.Platform.Data.EntityFrameworkCore.Mapping.Dept;

public class DepartmentAssignMap : EfEntityTypeConfiguration<DepartmentAssign>
{
    public override void Configure(EntityTypeBuilder<DepartmentAssign> builder)
    {
        builder.ToTable("sys_department_assignment");

        builder.Property(x => x.AdminId).IsRequired().HasMaxLength(100);
        builder.Property(x => x.DepartmentId).IsRequired().HasMaxLength(100);
            
        base.Configure(builder);
    }
}