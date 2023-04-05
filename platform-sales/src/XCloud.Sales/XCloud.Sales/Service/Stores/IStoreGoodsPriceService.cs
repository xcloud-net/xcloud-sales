using XCloud.Application.Extension;
using XCloud.Application.Mapper;
using XCloud.Core.Application.Entity;
using XCloud.Sales.Application;
using XCloud.Sales.Data;
using XCloud.Sales.Data.Domain.Stores;
using XCloud.Sales.Service.Catalog;

namespace XCloud.Sales.Service.Stores;

public interface IStoreGoodsPriceService : ISalesAppService
{
    Task<GoodsSpecCombinationDto[]> AttachStorePriceAsync(GoodsSpecCombinationDto[] data, string storeId);
    
    Task<StoreGoodsMappingDto[]> QueryManyByAsync(int[] combinationId, string storeId);
}

public class StoreGoodsPriceService : SalesAppService, IStoreGoodsPriceService
{
    private readonly IStoreGoodsMappingService _storeGoodsMappingService;
    private readonly ISalesRepository<StoreGoodsMapping> _repository;

    public StoreGoodsPriceService(IStoreGoodsMappingService storeGoodsMappingService,
        ISalesRepository<StoreGoodsMapping> repository)
    {
        _storeGoodsMappingService = storeGoodsMappingService;
        _repository = repository;
    }

    public async Task<GoodsSpecCombinationDto[]> AttachStorePriceAsync(GoodsSpecCombinationDto[] data, string storeId)
    {
        if (!data.Any())
            return data;
        if (string.IsNullOrWhiteSpace(storeId))
            throw new ArgumentNullException(nameof(storeId));

        var ids = data.Ids().ToArray();

        var db = await this._repository.GetDbContextAsync();

        var query = from storeMapping in db.Set<StoreGoodsMapping>().AsNoTracking()
            select new
            {
                storeMapping
            };

        query = query.Where(x => x.storeMapping.StoreId == storeId);
        query = query.Where(x => ids.Contains(x.storeMapping.GoodsCombinationId));

        var mappings = await query.TakeUpTo5000().ToArrayAsync();

        foreach (var m in data)
        {
            var items = mappings.Where(x => x.storeMapping.GoodsCombinationId == m.Id).Select(x => x.storeMapping)
                .ToArray();
            m.StoreGoodsMapping = this.ObjectMapper.MapArray<StoreGoodsMapping, StoreGoodsMappingDto>(items);
        }

        return data;
    }

    public async Task<StoreGoodsMappingDto[]> QueryManyByAsync(int[] combinationId, string storeId)
    {
        if (!combinationId.Any())
            throw new ArgumentNullException(nameof(combinationId));

        if (string.IsNullOrWhiteSpace(storeId))
            throw new ArgumentNullException(nameof(storeId));

        var db = await this._repository.GetDbContextAsync();

        var data = await db.Set<StoreGoodsMapping>()
            .Where(x => x.StoreId == storeId)
            .Where(x => combinationId.Contains(x.GoodsCombinationId))
            .OrderBy(x => x.CreationTime)
            .ToArrayAsync();

        return this.ObjectMapper.MapArray<StoreGoodsMapping, StoreGoodsMappingDto>(data);
    }
}