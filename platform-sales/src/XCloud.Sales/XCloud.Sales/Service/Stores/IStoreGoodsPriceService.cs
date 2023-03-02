using XCloud.Core.Application;
using XCloud.Sales.Application;
using XCloud.Sales.Data;
using XCloud.Sales.Data.Domain.Stores;

namespace XCloud.Sales.Service.Stores;

public interface IStoreGoodsPriceService : ISalesAppService
{
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