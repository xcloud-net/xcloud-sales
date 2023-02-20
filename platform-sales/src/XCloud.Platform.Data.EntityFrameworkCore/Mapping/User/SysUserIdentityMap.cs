using Microsoft.EntityFrameworkCore.Metadata.Builders;
using XCloud.Database.EntityFrameworkCore.Mapping;
using XCloud.Platform.Core.Domain.User;

namespace XCloud.Platform.Data.EntityFrameworkCore.Mapping.User;

public class SysUserIdentityMap : EfEntityTypeConfiguration<SysUserIdentity>
{
    public override void Configure(EntityTypeBuilder<SysUserIdentity> builder)
    {
        builder.ToTable("sys_user_identity");

        builder.Property(x => x.UserId).IsRequired().HasMaxLength(100);
        builder.Property(x => x.UserIdentity).IsRequired().HasMaxLength(50);

        builder.Property(x => x.Data).HasMaxLength(4000);

        builder.HasIndex(x => x.UserIdentity).IsUnique();
        builder.HasIndex(x => x.UserId);

        //mobile
        builder.Property(x => x.MobilePhone).HasMaxLength(20).HasDefaultValue(string.Empty);
        builder.Property(x => x.MobileAreaCode).HasMaxLength(10).HasDefaultValue(string.Empty);

        //email
        builder.Property(x => x.Email).HasMaxLength(100).HasDefaultValue(string.Empty);
            
        base.Configure(builder);

    }
}