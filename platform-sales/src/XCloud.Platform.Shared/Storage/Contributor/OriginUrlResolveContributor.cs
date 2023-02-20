using System.Threading.Tasks;

using Volo.Abp.DependencyInjection;
using XCloud.Platform.Shared.Dto;

namespace XCloud.Platform.Shared.Storage.Contributor;

[ExposeServices(typeof(IStorageUrlResolverContributor))]
public class OriginUrlResolveContributor : IStorageUrlResolverContributor, IScopedDependency
{
    public int Order => default;

    public bool Support(StorageMetaDto resourceData)
    {
        return !string.IsNullOrWhiteSpace(resourceData.Url);
    }

    public async Task<string> Resolve(StorageMetaDto resourceData, UrlResolveOption option)
    {
        await Task.CompletedTask;
        return resourceData.Url;
    }
}