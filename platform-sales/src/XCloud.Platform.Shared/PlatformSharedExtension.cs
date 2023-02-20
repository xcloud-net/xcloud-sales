
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using System.Threading.Tasks;

using Volo.Abp.Http.Client;
using XCloud.Platform.Shared.Dto;

namespace XCloud.Platform.Shared;

public static class PlatformSharedExtension
{
    public static StorageMetaDto Simplify(this StorageMetaDto dto)
    {
        dto.ResourceSize = default;
        dto.HashType = default;
        dto.ResourceHash = default;
        dto.ExtraData = default;
        return dto;
    }

    public static EntityTypeBuilder<T> MappingHasUserId<T>(this EntityTypeBuilder<T> builder, bool isRequired = true) where T : class, IHasUserId
    {
        var property = builder.Property(x => x.UserId).HasMaxLength(100);
        if (isRequired)
        {
            property.IsRequired();
        }

        return builder;
    }

    public static EntityTypeBuilder<T> MappingAccount<T>(this EntityTypeBuilder<T> builder) where T : class, IAccount
    {
        builder.Property(x => x.IdentityName).IsRequired().HasMaxLength(100);
        builder.Property(x => x.OriginIdentityName).HasMaxLength(100);

        builder.Property(x => x.NickName).HasMaxLength(30);
        builder.Property(x => x.PassWord).HasMaxLength(100);
        builder.Property(x => x.Avatar).HasMaxLength(2000);

        builder.HasIndex(x => x.IdentityName).IsUnique();

        return builder;
    }

    public static async Task<string> ResolveGatewayBaseAddressAsync(this IRemoteServiceConfigurationProvider remoteServiceConfigurationProvider)
    {
        var config = await remoteServiceConfigurationProvider.GetConfigurationOrDefaultAsync("Gateway");
        return config.BaseUrl;
    }
}