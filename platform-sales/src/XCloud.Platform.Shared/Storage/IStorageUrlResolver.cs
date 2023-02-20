using FluentAssertions;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using Volo.Abp.DependencyInjection;

using XCloud.Core.DataSerializer;
using XCloud.Core.Dto;
using XCloud.Platform.Shared.Dto;

namespace XCloud.Platform.Shared.Storage;

public interface IStorageUrlResolver
{
    StorageMetaDto DeserializeStorageDto(string resourceData);
    Task<ApiResponse<string>> ResolveUrlAsync(StorageMetaDto resourceData, UrlResolveOption option = null);
    Task<ApiResponse<string>> ResolveUrlAsync(string resourceData, UrlResolveOption option = null);
}

[ExposeServices(typeof(IStorageUrlResolver))]
public class StorageUrlResolver : IStorageUrlResolver, IScopedDependency
{
    private readonly IReadOnlyList<IStorageUrlResolverContributor> _storageUrlResolverContributors;

    private readonly IServiceProvider _serviceProvider;
    private readonly IJsonDataSerializer _jsonDataSerializer;
    private readonly ILogger _logger;

    public StorageUrlResolver(IServiceProvider serviceProvider, IJsonDataSerializer jsonDataSerializer, ILogger<StorageUrlResolver> logger)
    {
        this._serviceProvider = serviceProvider;
        this._jsonDataSerializer = jsonDataSerializer;
        this._logger = logger;

        this._storageUrlResolverContributors = serviceProvider.GetServices<IStorageUrlResolverContributor>().ToArray();
        this._storageUrlResolverContributors = this._storageUrlResolverContributors.OrderByDescending(x => x.Order).ToArray();
    }

    public StorageMetaDto DeserializeStorageDto(string resourceData)
    {
        if (string.IsNullOrWhiteSpace(resourceData))
            return new StorageMetaDto(StorageProviders.None);

        if (resourceData.StartsWith("http://", StringComparison.OrdinalIgnoreCase) ||
            resourceData.StartsWith("https://", StringComparison.OrdinalIgnoreCase))
            return new StorageMetaDto(StorageProviders.OriginUrl)
            {
                Url = resourceData
            };

        try
        {
            if (resourceData.TrimStart().StartsWith("{") &&
                resourceData.TrimEnd().EndsWith("}"))
                return this._jsonDataSerializer.DeserializeFromString<StorageMetaDto>(resourceData);
        }
        catch (Exception e)
        {
            this._logger.LogWarning(message: e.Message, exception: e);
        }
        return new StorageMetaDto(StorageProviders.None) { };
    }

    public async Task<ApiResponse<string>> ResolveUrlAsync(StorageMetaDto resourceData, UrlResolveOption option)
    {
        resourceData.Should().NotBeNull();

        var res = new ApiResponse<string>();

        foreach (var contrib in this._storageUrlResolverContributors)
        {
            if (contrib.Support(resourceData))
            {
                var url = await contrib.Resolve(resourceData, option);
                res.SetData(url);
                return res;
            }
        }

        await Task.CompletedTask;

        return res.SetError("err");
    }

    public async Task<ApiResponse<string>> ResolveUrlAsync(string resourceData, UrlResolveOption option)
    {
        var data = this.DeserializeStorageDto(resourceData);

        return await this.ResolveUrlAsync(data, option);
    }
}