
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using XCloud.Sales.Data.Domain.Users;

namespace XCloud.Sales.Data.Mapping.Users;

public class UserGradeMappingMap : SalesEntityTypeConfiguration<UserGradeMapping>
{
    public override void Configure(EntityTypeBuilder<UserGradeMapping> builder)
    {
        builder.ToTable(nameof(UserGradeMapping));

        builder.HasKey(x => x.Id);

        builder.Property(x => x.UserId);
        builder.Property(x => x.GradeId);
        builder.Property(x => x.StartTime);
        builder.Property(x => x.EndTime);

        builder.Property(x => x.CreationTime);

        base.Configure(builder);
    }
}