
using System.Threading.Tasks;

using Volo.Abp.DependencyInjection;
using Volo.Abp.Http.Client;
using XCloud.Platform.Shared.Dto;

namespace XCloud.Platform.Shared.Storage.Contributor;

[ExposeServices(typeof(IStorageUrlResolverContributor))]
public class AbpFsBlobResolveContributor : IStorageUrlResolverContributor, IScopedDependency
{
    private readonly IRemoteServiceConfigurationProvider remoteServiceConfigurationProvider;
    public AbpFsBlobResolveContributor(IRemoteServiceConfigurationProvider remoteServiceConfigurationProvider)
    {
        this.remoteServiceConfigurationProvider = remoteServiceConfigurationProvider;
    }

    public int Order => default;

    public bool Support(StorageMetaDto resourceData)
    {
        return resourceData.StorageProvider == StorageProviders.AbpFsBlobProvider1;
    }

    public async Task<string> Resolve(StorageMetaDto resourceData, UrlResolveOption option)
    {
        await Task.CompletedTask;

        var w = option?.Width ?? 0;
        var h = option?.Height ?? 0;

        var url = $"/api/platform/fs/file/{w}x{h}/{resourceData.ResourceKey}";

        return url;
    }
}