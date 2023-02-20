
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using XCloud.Sales.Data.Domain.Users;

namespace XCloud.Sales.Data.Mapping.Users;

public class UserGradeMap : SalesEntityTypeConfiguration<UserGrade>
{
    public override void Configure(EntityTypeBuilder<UserGrade> builder)
    {
        builder.ToTable(nameof(UserGrade));
        builder.HasKey(c => c.Id);
        builder.Property(u => u.Name).IsRequired().HasMaxLength(100);
        builder.Property(u => u.Description).HasMaxLength(1000);
        builder.Property(u => u.UserCount);
        builder.Property(x => x.Sort);

        base.Configure(builder);
    }
}