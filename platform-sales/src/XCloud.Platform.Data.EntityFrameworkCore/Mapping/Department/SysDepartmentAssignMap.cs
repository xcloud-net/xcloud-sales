using Microsoft.EntityFrameworkCore.Metadata.Builders;
using XCloud.Database.EntityFrameworkCore.Mapping;
using XCloud.Platform.Core.Domain.Department;

namespace XCloud.Platform.Data.EntityFrameworkCore.Mapping.Department;

public class SysDepartmentAssignMap : EfEntityTypeConfiguration<SysDepartmentAssign>
{
    public override void Configure(EntityTypeBuilder<SysDepartmentAssign> builder)
    {
        builder.ToTable("sys_department_assignment");

        builder.Property(x => x.AdminId).IsRequired().HasMaxLength(100);
        builder.Property(x => x.DepartmentId).IsRequired().HasMaxLength(100);
            
        base.Configure(builder);
    }
}