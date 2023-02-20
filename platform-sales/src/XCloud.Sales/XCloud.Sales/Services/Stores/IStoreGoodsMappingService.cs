using XCloud.Core.Application;
using XCloud.Database.EntityFrameworkCore.Extensions;
using XCloud.Sales.Data;
using XCloud.Sales.Data.Domain.Catalog;
using XCloud.Sales.Data.Domain.Stores;
using XCloud.Sales.Services.Catalog;

namespace XCloud.Sales.Services.Stores;

public interface IStoreGoodsMappingService: IXCloudApplicationService
{
    Task SaveGoodsStoreMappingAsync(SaveGoodsStoreMappingDto dto);
}

public class StoreGoodsMappingService: SalesAppService, IStoreGoodsMappingService
{
    private readonly ISalesRepository<Goods> _goodsRepository;
    private readonly IGoodsSpecCombinationService _goodsSpecCombinationService;

    public StoreGoodsMappingService(ISalesRepository<Goods> goodsRepository, 
        IGoodsSpecCombinationService goodsSpecCombinationService)
    {
        _goodsRepository = goodsRepository;
        _goodsSpecCombinationService = goodsSpecCombinationService;
    }

    public async Task SaveGoodsStoreMappingAsync(SaveGoodsStoreMappingDto dto)
    {
        if (dto == null)
            throw new ArgumentNullException(nameof(dto));
        if (dto.GoodsCombinationId <= 0)
            throw new ArgumentException(nameof(dto.GoodsCombinationId));
        if (dto.StoreIds == null)
            throw new ArgumentNullException(nameof(dto.StoreIds));

        var db = await _goodsRepository.GetDbContextAsync();

        var combination = await this._goodsSpecCombinationService.QueryByIdAsync(dto.GoodsCombinationId);
        if (combination == null)
            throw new EntityNotFoundException(nameof(combination));

        var mappings = dto.StoreIds.Select(x => new StoreGoodsMapping()
        {
            GoodsCombinationId = combination.Id,
            StoreId = x
        }).ToArray();

        var set = db.Set<StoreGoodsMapping>();

        var onSales = await set.Where(x => x.GoodsCombinationId == dto.GoodsCombinationId).ToArrayAsync();

        var toRemove = onSales.NotInBy(mappings, x => x.StoreId).ToArray();
        var toInsert = mappings.NotInBy(onSales, x => x.StoreId).ToArray();
        
        foreach (var m in toInsert)
        {
            m.Id = GuidGenerator.CreateGuidString();
        }
        
        if (toRemove.Any())
            set.RemoveRange(toRemove);

        if (toInsert.Any())
            set.AddRange(toInsert);

        await db.TrySaveChangesAsync();
    }
}