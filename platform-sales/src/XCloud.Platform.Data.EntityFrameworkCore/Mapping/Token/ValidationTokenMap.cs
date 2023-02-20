using Microsoft.EntityFrameworkCore.Metadata.Builders;
using XCloud.Database.EntityFrameworkCore.Mapping;
using XCloud.Platform.Core.Domain.Token;

namespace XCloud.Platform.Data.EntityFrameworkCore.Mapping.Token;

public class ValidationTokenMap : EfEntityTypeConfiguration<ValidationToken>
{
    public override void Configure(EntityTypeBuilder<ValidationToken> builder)
    {
        builder.ToTable("sys_validation_token");

        builder.Property(x => x.Token).IsRequired().HasMaxLength(300);

        builder.HasIndex(x => x.Token).IsUnique();
            
        base.Configure(builder);
    }
}