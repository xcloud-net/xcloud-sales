using XCloud.Database.EntityFrameworkCore.Extensions;
using XCloud.Platform.Shared.Storage;
using XCloud.Sales.Application;
using XCloud.Sales.Data;
using XCloud.Sales.Data.Domain.Common;

namespace XCloud.Sales.Service.Common;

public interface IPagesService : ISalesPagingStringAppService<Pages, PagesDto, QueryPagesInput>
{
    Task SetPageContentAsync(SetPageContentInput dto);

    Task<PagesDto> QueryPagesBySeoNameAsync(string seoName);

    Task UpdatePagesStatusAsync(UpdatePagesStatusInput dto);

    Task<PagesDto[]> AttachDataAsync(PagesDto[] data, AttachPageDataInput dto);
}

public class PagesService : SalesPagingStringAppService<Pages, PagesDto, QueryPagesInput>, IPagesService
{
    private readonly ISalesRepository<Pages> _salesRepository;
    private readonly IStorageUrlResolver _storageUrlResolver;

    public PagesService(ISalesRepository<Pages> salesRepository, IStorageUrlResolver storageUrlResolver) : base(
        salesRepository)
    {
        this._salesRepository = salesRepository;
        this._storageUrlResolver = storageUrlResolver;
    }

    private string SerializeBodyContent(object obj)
    {
        if (obj == null)
            throw new ArgumentNullException(nameof(obj));

        var json = this.JsonDataSerializer.SerializeToString(obj);
        return json;
    }

    private string NormalizeBodyContent(string json)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(json))
                return this.JsonDataSerializer.SerializeToString(new { });

            var data = this.JsonDataSerializer.DeserializeFromString<object>(json);
            return this.JsonDataSerializer.SerializeToString(data);
        }
        catch (Exception e)
        {
            this.Logger.LogError(exception: e, message: e.Message);
            return this.JsonDataSerializer.SerializeToString(new { error = true, error_data = json });
        }
    }

    private string NormalizePagesSeoName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            return string.Empty;

        var ignoreChars = new[] { '/', '\\', '@', '#', '$', '&', '!', ' ' };

        name = string.Join(String.Empty, name.Where(x => !ignoreChars.Contains(x)));

        return name.ToLower();
    }

    public async Task<PagesDto[]> AttachDataAsync(PagesDto[] data, AttachPageDataInput dto)
    {
        if (!data.Any())
            return data;

        if (dto.CoverImage)
        {
            foreach (var m in data)
            {
                m.Cover = this._storageUrlResolver.DeserializeStorageDto(m.CoverImageResourceData);
            }
        }

        await Task.CompletedTask;
        return data;
    }

    protected override async Task CheckBeforeInsertAsync(PagesDto dto)
    {
        await Task.CompletedTask;

        if (string.IsNullOrWhiteSpace(dto.Title))
            throw new UserFriendlyException("title is required");
    }

    protected override async Task InitBeforeInsertAsync(Pages entity)
    {
        await Task.CompletedTask;

        entity.Id = this.GuidGenerator.CreateGuidString();
        entity.BodyContent = string.Empty;
        entity.IsPublished = false;
        entity.IsDeleted = false;
        entity.CreationTime = this.Clock.Now;
        entity.PublishedTime = entity.CreationTime;
        entity.SeoName = this.NormalizePagesSeoName(entity.SeoName);

        if (await this.CheckSeoNameIsExistAsync(entity.SeoName))
            throw new UserFriendlyException("page address is already exist");
    }

    protected override async Task CheckBeforeUpdateAsync(PagesDto dto)
    {
        await Task.CompletedTask;

        if (string.IsNullOrWhiteSpace(dto.Title))
            throw new UserFriendlyException("title is required");
    }

    protected override async Task ModifyFieldsForUpdateAsync(Pages entity, PagesDto dto)
    {
        await Task.CompletedTask;

        entity.Title = dto.Title;
        entity.Description = dto.Description;
        entity.CoverImageResourceData = dto.CoverImageResourceData;
        entity.SeoName = dto.SeoName;
        entity.SeoName = this.NormalizePagesSeoName(entity.SeoName);

        if (await this.CheckSeoNameIsExistAsync(entity.SeoName, entity.Id))
            throw new UserFriendlyException("page address is already exist");
    }

    private async Task<bool> CheckSeoNameIsExistAsync(string seoName, string exceptId = null)
    {
        var db = await this.Repository.GetDbContextAsync();

        var query = db.Set<Pages>().IgnoreQueryFilters().AsNoTracking();

        query = query.Where(x => x.SeoName == seoName);

        if (!string.IsNullOrWhiteSpace(exceptId))
            query = query.Where(x => x.Id != exceptId);

        return await query.AnyAsync();
    }

    public async Task SetPageContentAsync(SetPageContentInput dto)
    {
        if (string.IsNullOrWhiteSpace(dto.Id))
            throw new ArgumentNullException(nameof(dto.Id));

        var db = await this._salesRepository.GetDbContextAsync();
        var set = db.Set<Pages>();

        var page = await set.FirstOrDefaultAsync(x => x.Id == dto.Id);
        if (page == null)
            throw new EntityNotFoundException(nameof(SetPageContentAsync));

        page.BodyContent = this.NormalizeBodyContent(dto.Content);

        await db.TrySaveChangesAsync();
    }

    protected override async Task<IQueryable<Pages>> GetFilteredQueryableAsync(IQueryable<Pages> query,
        QueryPagesInput dto)
    {
        await Task.CompletedTask;

        if (dto.IsPublished != null)
            query = query.Where(x => x.IsPublished == dto.IsPublished.Value);

        if (dto.IsDeleted != null)
            query = query.Where(x => x.IsDeleted == dto.IsDeleted.Value);

        if (!string.IsNullOrWhiteSpace(dto.Keyword))
            query = query.Where(x =>
                x.Title.Contains(dto.Keyword) ||
                x.Description.Contains(dto.Keyword));

        return query;
    }

    protected override async Task<IOrderedQueryable<Pages>> GetOrderedQueryableAsync(IQueryable<Pages> query,
        QueryPagesInput dto)
    {
        await Task.CompletedTask;

        if (dto.SortForAdmin ?? false)
        {
            return query.OrderBy(x => x.IsDeleted).ThenByDescending(x => x.CreationTime);
        }
        else
        {
            return query.OrderByDescending(x => x.CreationTime);
        }
    }

    public async Task<PagesDto> QueryPagesBySeoNameAsync(string seoName)
    {
        if (string.IsNullOrWhiteSpace(seoName))
            return null;

        seoName = this.NormalizePagesSeoName(seoName);

        var query = await this._salesRepository.GetQueryableAsync();
        var page = await query.OrderBy(x => x.CreationTime)
            .FirstOrDefaultAsync(x => x.SeoName == seoName);
        if (page == null)
            return null;

        var pageDto = this.ObjectMapper.Map<Pages, PagesDto>(page);
        return pageDto;
    }

    public async Task UpdatePagesStatusAsync(UpdatePagesStatusInput dto)
    {
        var db = await this._salesRepository.GetDbContextAsync();

        var page = await db.Set<Pages>().IgnoreQueryFilters().FirstOrDefaultAsync(x => x.Id == dto.Id);
        if (page == null)
            throw new EntityNotFoundException(nameof(UpdatePagesStatusAsync));

        if (dto.IsDeleted != null)
            page.IsDeleted = dto.IsDeleted.Value;

        if (dto.IsPublished != null)
            page.IsPublished = dto.IsPublished.Value;

        await db.TrySaveChangesAsync();
    }
}