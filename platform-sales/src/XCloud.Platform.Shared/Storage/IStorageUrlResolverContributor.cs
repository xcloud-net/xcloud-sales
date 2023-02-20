using System.Threading.Tasks;
using XCloud.Platform.Shared.Dto;

namespace XCloud.Platform.Shared.Storage;

public interface IStorageUrlResolverContributor
{
    int Order { get; }
    bool Support(StorageMetaDto resourceData);
    Task<string> Resolve(StorageMetaDto resourceData, UrlResolveOption option);
}