using XCloud.Application.Extension;
using XCloud.Core.Application.Entity;
using XCloud.Core.Dto;
using XCloud.Core.Helper;
using XCloud.Database.EntityFrameworkCore.Extensions;
using XCloud.Sales.Application;
using XCloud.Sales.Data;
using XCloud.Sales.Data.Domain.Catalog;

namespace XCloud.Sales.Service.Catalog;

public interface IGoodsCollectionService : ISalesAppService
{
    Task<GoodsCollectionDto> QueryByIdAsync(string collectionId);

    [Obsolete("won't load combination data in the next version,pls load manually")]
    Task<GoodsCollectionDto[]> AttachCollectionItemsDataAsync(GoodsCollectionDto[] data);

    Task<GoodsCollectionItemDto[]> AttachDataAsync(GoodsCollectionItemDto[] data,
        AttachCollectionItemDataInput dto);

    Task<GoodsCollectionDto[]> QueryAllAsync();

    Task<ApiResponse<GoodsCollection>> InsertAsync(GoodsCollection input);

    Task<ApiResponse<GoodsCollection>> UpdateAsync(GoodsCollection dto);

    Task SoftDeleteCollectionAsync(string id);

    Task RemoveCollectionItemByIdAsync(string id);

    Task<ApiResponse<GoodsCollectionItem>> InsertItemAsync(GoodsCollectionItemDto dto);

    Task<GoodsCollectionItemDto[]> QueryItemsByCollectionIdAsync(string collectionId);
}

public class GoodsCollectionService : SalesAppService, IGoodsCollectionService
{
    private readonly ISalesRepository<GoodsCollection> _salesRepository;

    public GoodsCollectionService(ISalesRepository<GoodsCollection> salesRepository)
    {
        this._salesRepository = salesRepository;
    }

    public async Task<GoodsCollectionDto> QueryByIdAsync(string collectionId)
    {
        if (string.IsNullOrWhiteSpace(collectionId))
            throw new ArgumentNullException(nameof(collectionId));

        var entity = await this._salesRepository.QueryOneAsync(x => x.Id == collectionId);
        if (entity == null)
            return null;

        var collection = this.ObjectMapper.Map<GoodsCollection, GoodsCollectionDto>(entity);
        return collection;
    }

    public async Task<GoodsCollectionItemDto[]> QueryItemsByCollectionIdAsync(string collectionId)
    {
        var db = await _salesRepository.GetDbContextAsync();

        var items = await db.Set<GoodsCollectionItem>().Where(x => x.CollectionId == collectionId).ToArrayAsync();

        var data = items.Select(x => ObjectMapper.Map<GoodsCollectionItem, GoodsCollectionItemDto>(x)).ToArray();

        return data;
    }

    public async Task<GoodsCollectionItemDto[]> AttachDataAsync(GoodsCollectionItemDto[] data,
        AttachCollectionItemDataInput dto)
    {
        if (ValidateHelper.IsEmptyCollection(data))
            return data;

        var db = await this._salesRepository.GetDbContextAsync();

        if (dto.Combination)
        {
            var combinationIds = data.Select(x => x.GoodsSpecCombinationId).Distinct().ToArray();
            var combinationList = await db.Set<GoodsSpecCombination>().WhereIdIn(combinationIds).ToArrayAsync();

            foreach (var m in data)
            {
                var combination = combinationList.FirstOrDefault(x => x.Id == m.GoodsSpecCombinationId);
                if (combination == null)
                    continue;

                m.GoodsSpecCombination =
                    this.ObjectMapper.Map<GoodsSpecCombination, GoodsSpecCombinationDto>(combination);
            }
        }

        return data;
    }

    public async Task<GoodsCollectionDto[]> AttachCollectionItemsDataAsync(GoodsCollectionDto[] data)
    {
        if (!data.Any())
            return data;

        var db = await _salesRepository.GetDbContextAsync();

        var query = from item in db.Set<GoodsCollectionItem>().AsNoTracking()
            join combination in db.Set<GoodsSpecCombination>().AsNoTracking()
                on item.GoodsSpecCombinationId equals combination.Id
            join goods in db.Set<Goods>().AsNoTracking()
                on combination.GoodsId equals goods.Id
            select new
            {
                item,
                combination,
            };

        var ids = data.Ids().ToArray();
        query = query.Where(x => ids.Contains(x.item.CollectionId));

        var res = await query.ToArrayAsync();

        foreach (var m in data)
        {
            var items = res.Where(x => x.item.CollectionId == m.Id).ToArray();
            var list = new List<GoodsCollectionItemDto>();

            foreach (var d in items)
            {
                var dto = ObjectMapper.Map<GoodsCollectionItem, GoodsCollectionItemDto>(d.item);
                dto.GoodsSpecCombination =
                    ObjectMapper.Map<GoodsSpecCombination, GoodsSpecCombinationDto>(d.combination);

                list.Add(dto);
            }

            m.Items = list.ToArray();
        }

        return data;
    }

    public async Task<GoodsCollectionDto[]> QueryAllAsync()
    {
        var db = await _salesRepository.GetDbContextAsync();

        var data = await db.Set<GoodsCollection>().AsNoTracking()
            .OrderByDescending(x => x.LastModificationTime)
            .ThenByDescending(x => x.CreationTime)
            .TakeUpTo5000().ToArrayAsync();

        var dtos = data.Select(x => ObjectMapper.Map<GoodsCollection, GoodsCollectionDto>(x)).ToArray();

        return dtos;
    }

    public async Task<ApiResponse<GoodsCollection>> InsertAsync(GoodsCollection input)
    {
        if (input == null)
            throw new ArgumentNullException(nameof(input));
        if (string.IsNullOrWhiteSpace(input.Name))
            return new ApiResponse<GoodsCollection>().SetError("name is required");

        var db = await _salesRepository.GetDbContextAsync();

        input.Id = GuidGenerator.CreateGuidString();
        input.CreationTime = Clock.Now;

        db.Set<GoodsCollection>().Add(input);

        await db.SaveChangesAsync();

        return new ApiResponse<GoodsCollection>(input);
    }

    public async Task<ApiResponse<GoodsCollection>> UpdateAsync(GoodsCollection dto)
    {
        if (dto == null)
            throw new ArgumentNullException(nameof(dto));
        if (string.IsNullOrWhiteSpace(dto.Name))
            return new ApiResponse<GoodsCollection>().SetError("name is required");

        var db = await _salesRepository.GetDbContextAsync();

        var entity = await db.Set<GoodsCollection>().FirstOrDefaultAsync(x => x.Id == dto.Id);
        if (entity == null)
            throw new EntityNotFoundException(nameof(UpdateAsync));

        entity.Name = dto.Name;
        entity.Description = dto.Description;
        entity.Keywords = dto.Keywords;
        entity.LastModificationTime = Clock.Now;

        await db.TrySaveChangesAsync();

        return new ApiResponse<GoodsCollection>(entity);
    }

    public async Task SoftDeleteCollectionAsync(string id)
    {
        if (string.IsNullOrWhiteSpace(id))
            throw new ArgumentNullException(nameof(id));

        var db = await _salesRepository.GetDbContextAsync();

        var entity = await db.Set<GoodsCollection>().FirstOrDefaultAsync(x => x.Id == id);

        if (entity == null)
            return;

        entity.IsDeleted = true;
        entity.DeletionTime = Clock.Now;

        await db.TrySaveChangesAsync();
    }

    public async Task<ApiResponse<GoodsCollectionItem>> InsertItemAsync(GoodsCollectionItemDto dto)
    {
        if (string.IsNullOrWhiteSpace(dto.CollectionId))
            throw new ArgumentNullException(nameof(dto.CollectionId));
        if (dto.GoodsSpecCombinationId <= 0)
            throw new ArgumentException(nameof(dto.GoodsSpecCombinationId));

        var db = await _salesRepository.GetDbContextAsync();

        var query = from combination in db.Set<GoodsSpecCombination>().AsNoTracking()
            join goods in db.Set<Goods>().AsNoTracking()
                on combination.GoodsId equals goods.Id
            select new { combination, goods };

        var goodsExists = await query.FirstOrDefaultAsync(x =>
            x.combination.Id == dto.GoodsSpecCombinationId && x.goods.Id == dto.GoodsId);
        if (goodsExists == null)
            return new ApiResponse<GoodsCollectionItem>().SetError("goods not exists");

        dto.GoodsId = goodsExists.goods.Id;

        var set = db.Set<GoodsCollectionItem>();

        var item = await set.FirstOrDefaultAsync(x =>
            x.CollectionId == dto.CollectionId &&
            x.GoodsSpecCombinationId == dto.GoodsSpecCombinationId);

        if (item != null)
            return new ApiResponse<GoodsCollectionItem>().SetError("goods already exist");

        var entity = ObjectMapper.Map<GoodsCollectionItemDto, GoodsCollectionItem>(dto);
        entity.Id = GuidGenerator.CreateGuidString();
        entity.CreationTime = Clock.Now;
        set.Add(entity);

        await db.SaveChangesAsync();

        return new ApiResponse<GoodsCollectionItem>(entity);
    }

    public async Task RemoveCollectionItemByIdAsync(string id)
    {
        if (string.IsNullOrWhiteSpace(id))
            throw new ArgumentNullException(nameof(id));

        var db = await _salesRepository.GetDbContextAsync();

        var set = db.Set<GoodsCollectionItem>();

        var item = await set.FirstOrDefaultAsync(x => x.Id == id);

        if (item == null)
            return;

        set.Remove(item);

        await db.TrySaveChangesAsync();
    }
}