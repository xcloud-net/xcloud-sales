using XCloud.Sales.Application;
using XCloud.Sales.Data;
using XCloud.Sales.Data.Domain.Catalog;

namespace XCloud.Sales.Service.Catalog;

public interface IGoodsStockService : ISalesAppService
{
    Task AdjustCombinationStockAsync(int combinationId, int quantityOffset);

    Task SetCombinationStockAsync(int combinationId, int stock);
}

public class GoodsStockService : SalesAppService, IGoodsStockService
{
    private readonly ISalesRepository<GoodsSpecCombination> _combinationRepository;

    public GoodsStockService(ISalesRepository<GoodsSpecCombination> combinationRepository)
    {
        this._combinationRepository = combinationRepository;
    }

    public async Task SetCombinationStockAsync(int combinationId, int stock)
    {
        if (combinationId <= 0)
            throw new ArgumentNullException(nameof(combinationId));

        var entity = await this._combinationRepository.QueryOneAsync(x => x.Id == combinationId);
        if (entity == null)
            throw new EntityNotFoundException(nameof(SetCombinationStockAsync));

        entity.StockQuantity = stock;
        entity.LastModificationTime = this.Clock.Now;

        await this._combinationRepository.UpdateAsync(entity);
    }

    public virtual async Task AdjustCombinationStockAsync(
        int combinationId,
        int quantityOffset)
    {
        if (combinationId <= 0)
            throw new ArgumentNullException(nameof(combinationId));

        if (quantityOffset == 0)
            throw new ArgumentNullException(nameof(quantityOffset));

        var db = await this._combinationRepository.GetDbContextAsync();

        var set = db.Set<GoodsSpecCombination>();

        var entity = await set.FirstOrDefaultAsync(x => x.Id == combinationId);

        if (entity == null)
            throw new EntityNotFoundException("goods combination not found");

        entity.StockQuantity += quantityOffset;
        entity.LastModificationTime = this.Clock.Now;

        if (entity.StockQuantity <= 0)
        {
            Logger.LogWarning($"stock quantity is running out:{entity.Id}");
        }

        await this._combinationRepository.UpdateAsync(entity);
    }
}