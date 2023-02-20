using System.Threading.Tasks;
using FluentAssertions;
using XCloud.Application.Storage;
using XCloud.Core.Application;
using XCloud.Core.Cache;
using XCloud.Core.Dto;
using XCloud.Platform.Core.Application;
using XCloud.Platform.Core.Domain.Storage;
using XCloud.Platform.Data.Database;

namespace XCloud.Platform.Common.Application.Service.Storage;

public interface IStorageMetaService : IXCloudApplicationService
{
    Task<string> GetContentTypeOrNullAsync(string key);
    
    Task<StorageResourceMeta> TrySaveResourceMetaAsync(StorageResourceMeta model);
    
    Task<StorageResourceMeta> GetFileByKeyAsync(string fileKey);
    
    Task<StorageResourceMeta> GetFileByKeyAsync(string fileKey, CachePolicy option);
    
    Task<PagedResponse<StorageResourceMetaDto>> QueryPagingAsync(QueryStoragePagingInput dto);
}

public class StorageMetaService : PlatformApplicationService, IStorageMetaService
{
    private readonly IPlatformRepository<StorageResourceMeta> _uploadRepo;
    private readonly StorageHelper _storageHelper;

    public StorageMetaService(
        IPlatformRepository<StorageResourceMeta> uploadRepo,
        StorageHelper storageHelper)
    {
        this._uploadRepo = uploadRepo;
        this._storageHelper = storageHelper;
    }

    public async Task<PagedResponse<StorageResourceMetaDto>> QueryPagingAsync(QueryStoragePagingInput dto)
    {
        if (dto == null)
            throw new ArgumentNullException(nameof(dto));
        
        var db = await this._uploadRepo.GetDbContextAsync();

        var query = db.Set<StorageResourceMeta>().AsNoTracking();

        if (!string.IsNullOrWhiteSpace(dto.FileExtension))
        {
            var ext = this._storageHelper.GetNormalizedFileExtension(dto.FileExtension);
            query = query.Where(x => x.FileExtension == ext);
        }

        if (!string.IsNullOrWhiteSpace(dto.ContentType))
            query = query.Where(x => x.ContentType == dto.ContentType);
        
        if (!string.IsNullOrWhiteSpace(dto.StorageProvider))
            query = query.Where(x => x.StorageProvider == dto.StorageProvider);

        if (dto.MinSize != null)
            query = query.Where(x => x.ResourceSize >= dto.MinSize.Value);
        
        if (dto.MaxSize != null)
            query = query.Where(x => x.ResourceSize <= dto.MaxSize.Value);

        if (dto.StartTime != null)
            query = query.Where(x => x.CreationTime >= dto.StartTime.Value);

        if (dto.EndTime != null)
            query = query.Where(x => x.CreationTime <= dto.EndTime.Value);
        
        var count = 0;
        if (!dto.SkipCalculateTotalCount)
            count = await query.CountAsync();

        var data = await query
            .OrderByDescending(x => x.CreationTime)
            .PageBy(dto.AsAbpPagedRequestDto())
            .ToArrayAsync();

        var items = data.Select(x => this.ObjectMapper.Map<StorageResourceMeta, StorageResourceMetaDto>(x)).ToArray();

        return new PagedResponse<StorageResourceMetaDto>(items, dto, count);
    }

    public async Task<string> GetContentTypeOrNullAsync(string key)
    {
        var contentType = this._storageHelper.ResolveContentTypeOrNull(key);

        if (string.IsNullOrWhiteSpace(contentType))
        {
            var res = await this.GetFileByKeyAsync(key, new CachePolicy() { Cache = true });
            if (res != null)
                contentType = res.ContentType;
        }

        return contentType;
    }

    public async Task<StorageResourceMeta> TrySaveResourceMetaAsync(StorageResourceMeta model)
    {
        var previousUploaded = await this.GetPreviousUploadedResource(model);

        if (previousUploaded != null)
        {
            await this.PlusResourceMetaUploadTimes(previousUploaded);

            await this.GetFileByKeyAsync(previousUploaded.ResourceKey, new CachePolicy() { Refresh = true });

            return previousUploaded;
        }
        else
        {
            await this.InsertResourceMeta(model);

            await this.GetFileByKeyAsync(model.ResourceKey, new CachePolicy() { Refresh = true });

            return model;
        }
    }

    async Task<StorageResourceMeta> GetPreviousUploadedResource(StorageResourceMeta model)
    {
        var previousUploaded = await _uploadRepo.QueryOneAsync(x =>
            x.StorageProvider == model.StorageProvider &&
            x.HashType == model.HashType &&
            x.ResourceHash == model.ResourceHash &&
            x.ResourceSize == model.ResourceSize &&
            x.ResourceKey == model.ResourceKey);

        return previousUploaded;
    }

    private async Task PlusResourceMetaUploadTimes(StorageResourceMeta previousUploaded)
    {
        ++previousUploaded.UploadTimes;
        previousUploaded.LastModificationTime = this.Clock.Now;
        await this._uploadRepo.UpdateNowAsync(previousUploaded);
    }

    async Task InsertResourceMeta(StorageResourceMeta model)
    {
        //保存到数据库
        model.Id = this.GuidGenerator.CreateGuidString();
        model.CreationTime = this.Clock.Now;

        await _uploadRepo.InsertNowAsync(model);
    }

    public async Task<StorageResourceMeta> GetFileByKeyAsync(string fileKey)
    {
        fileKey.Should().NotBeNullOrEmpty();

        var previousUploaded = await _uploadRepo.QueryOneAsync(x => x.ResourceKey == fileKey);
        return previousUploaded;
    }

    public async Task<StorageResourceMeta> GetFileByKeyAsync(string fileKey, CachePolicy option)
    {
        var key = $"storage.meta.by-key:{fileKey}";

        var data = await this.CacheProvider.ExecuteWithPolicyAsync(
            () => this.GetFileByKeyAsync(fileKey),
            new XCloud.Core.Cache.CacheOption<StorageResourceMeta>(key, TimeSpan.FromMinutes(1000)),
            option);

        return data;
    }

}