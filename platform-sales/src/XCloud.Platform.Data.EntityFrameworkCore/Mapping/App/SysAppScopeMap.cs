using Microsoft.EntityFrameworkCore.Metadata.Builders;
using XCloud.Database.EntityFrameworkCore.Mapping;
using XCloud.Platform.Core.Domain.App;

namespace XCloud.Platform.Data.EntityFrameworkCore.Mapping.App;

public class SysAppScopeMap : EfEntityTypeConfiguration<SysAppScope>
{
    public override void Configure(EntityTypeBuilder<SysAppScope> builder)
    {
        builder.ToTable("sys_app_scope");

        builder.Property(x => x.SubjectId).IsRequired().HasMaxLength(100);
        builder.Property(x => x.SubjectType).IsRequired().HasMaxLength(100);
        builder.Property(x => x.AppKey).IsRequired().HasMaxLength(100);
            
        base.Configure(builder);
    }
}