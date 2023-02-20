using Microsoft.EntityFrameworkCore.Metadata.Builders;
using XCloud.Database.EntityFrameworkCore.Mapping;
using XCloud.Platform.Core.Domain.User;

namespace XCloud.Platform.Data.EntityFrameworkCore.Mapping.User;

public class SysUserRealNameMap : EfEntityTypeConfiguration<SysUserRealName>
{
    public override void Configure(EntityTypeBuilder<SysUserRealName> builder)
    {
        builder.ToTable("sys_user_realname");

        builder.Property(x => x.IdCardIdentity).IsRequired().HasMaxLength(100);
        builder.HasIndex(x => x.IdCardIdentity).IsUnique();

        builder.Property(x => x.UserId).IsRequired().HasMaxLength(100);
        builder.HasIndex(x => x.UserId);

        builder.Property(x => x.Data).HasMaxLength(4000);

        //id card
        builder.Property(x => x.IdCard).IsRequired().HasMaxLength(100);
        builder.Property(x => x.IdCardRealName).HasMaxLength(20);

        builder.Property(x => x.IdCardFrontUrl).HasMaxLength(1000);
        builder.Property(x => x.IdCardBackUrl).HasMaxLength(1000);
        builder.Property(x => x.IdCardHandUrl).HasMaxLength(1000);

        builder.Property(x => x.ConfirmerId).HasMaxLength(100);
            
        base.Configure(builder);

    }
}