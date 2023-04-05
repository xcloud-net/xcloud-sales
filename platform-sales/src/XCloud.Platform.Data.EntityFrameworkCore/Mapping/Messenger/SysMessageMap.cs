using Microsoft.EntityFrameworkCore.Metadata.Builders;
using XCloud.Database.EntityFrameworkCore.Mapping;
using XCloud.Platform.Core.Domain.Messenger;

namespace XCloud.Platform.Data.EntityFrameworkCore.Mapping.Messenger;

public class SysMessageMap : EfEntityTypeConfiguration<SysMessage>
{
    public override void Configure(EntityTypeBuilder<SysMessage> builder)
    {
        base.Configure(builder);
        
        builder.ToTable("sys_message");
        
        builder.Property(x => x.Message).HasMaxLength(1000);
        builder.Property(x => x.MessageType).HasMaxLength(100);
        builder.Property(x => x.Data);
        builder.Property(x => x.CreationTime);
    }
}