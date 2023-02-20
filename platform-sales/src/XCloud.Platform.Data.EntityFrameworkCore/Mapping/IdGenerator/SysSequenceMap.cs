using Microsoft.EntityFrameworkCore.Metadata.Builders;
using XCloud.Database.EntityFrameworkCore.Mapping;
using XCloud.Platform.Core.Domain.IdGenerator;

namespace XCloud.Platform.Data.EntityFrameworkCore.Mapping.IdGenerator;

public class SysSequenceMap : EfEntityTypeConfiguration<SysSequence>
{
    public override void Configure(EntityTypeBuilder<SysSequence> builder)
    {
        builder.ToTable("sys_sequence");
        builder.Property(x => x.Category).IsRequired().HasMaxLength(100);
        builder.Property(x => x.Description).HasMaxLength(500);
        builder.Property(x => x.NextId).IsConcurrencyToken();

        builder.HasIndex(x => x.Category).IsUnique();
            
        base.Configure(builder);
    }
}