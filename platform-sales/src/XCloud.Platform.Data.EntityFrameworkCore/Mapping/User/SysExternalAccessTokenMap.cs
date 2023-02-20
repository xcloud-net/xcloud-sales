using Microsoft.EntityFrameworkCore.Metadata.Builders;
using XCloud.Database.EntityFrameworkCore.Mapping;
using XCloud.Platform.Core.Domain.User;

namespace XCloud.Platform.Data.EntityFrameworkCore.Mapping.User;

public class SysExternalAccessTokenMap : EfEntityTypeConfiguration<SysExternalAccessToken>
{
    public override void Configure(EntityTypeBuilder<SysExternalAccessToken> builder)
    {
        builder.ToTable("sys_external_access_token");

        builder.Property(x => x.Platform).IsRequired().HasMaxLength(100);
        builder.Property(x => x.AppId).IsRequired().HasMaxLength(100);

        builder.Property(x => x.UserId).HasMaxLength(100);
        builder.Property(x => x.Scope).HasMaxLength(100);
        builder.Property(x => x.GrantType).HasMaxLength(100);

        builder.Property(x => x.AccessTokenType);
        builder.Property(x => x.AccessToken).IsRequired().HasMaxLength(1000);

        builder.Property(x => x.RefreshToken).HasMaxLength(1000);
        builder.Property(x => x.ExpiredAt);

        base.Configure(builder);
    }
}