using XCloud.Redis;
using XCloud.Sales.Application;
using XCloud.Sales.Data;
using XCloud.Sales.Data.Domain.WarehouseStock;
using XCloud.Sales.Service.Catalog;

namespace XCloud.Sales.Service.WarehouseStock;

public interface IStockProcessingService : ISalesAppService
{
    Task UpdateGoodsStockFromWarehouseStockAsync(int combinationId);
    
    Task RevertStockDeductionAsync(string orderId);
    
    Task DeductStockAsync(DeductStockInput dto);
}

public class StockProcessingService : SalesAppService, IStockProcessingService
{
    private readonly ISalesRepository<Stock> _salesRepository;
    private readonly ISalesRepository<StockItem> _stockItemRepository;
    private readonly IStockUsageHistoryService _stockUsageHistoryService;
    private readonly IGoodsStockService _goodsStockService;
    private readonly IStockService _stockService;

    public StockProcessingService(ISalesRepository<Stock> salesRepository,
        IStockUsageHistoryService stockUsageHistoryService,
        IGoodsStockService goodsStockService, IStockService stockService,
        ISalesRepository<StockItem> stockItemRepository)
    {
        _stockUsageHistoryService = stockUsageHistoryService;
        _goodsStockService = goodsStockService;
        _stockService = stockService;
        _stockItemRepository = stockItemRepository;
        this._salesRepository = salesRepository;
    }

    public async Task UpdateGoodsStockFromWarehouseStockAsync(int combinationId)
    {
        var count = await this._stockService.QueryGoodsCombinationWarehouseStockAsync(combinationId);
        await this._goodsStockService.SetCombinationStockAsync(combinationId, count);
    }

    public async Task RevertStockDeductionAsync(string orderId)
    {
        var db = await this._salesRepository.GetDbContextAsync();
        throw new NotImplementedException();
    }

    private async Task<int> DeductStockAsync(string orderItemId, string stockItemId, int quantity)
    {
        if (string.IsNullOrWhiteSpace(orderItemId))
            throw new ArgumentNullException(nameof(orderItemId));

        if (string.IsNullOrWhiteSpace(stockItemId))
            throw new ArgumentNullException(nameof(stockItemId));

        if (quantity <= 0)
            throw new ArgumentNullException(nameof(quantity));

        var db = await this._salesRepository.GetDbContextAsync();

        var entity = await db.Set<StockItem>().FirstOrDefaultAsync(x => x.Id == stockItemId);

        if (entity == null)
            throw new EntityNotFoundException(nameof(entity));

        var availableQuantity = entity.Quantity - entity.DeductQuantity;

        if (entity.RuningOut || availableQuantity <= 0)
            return 0;

        var deductQuantity = Math.Min(quantity, availableQuantity);

        entity.DeductQuantity += deductQuantity;
        entity.RuningOut = entity.Quantity <= entity.DeductQuantity;
        entity.LastModificationTime = this.Clock.Now;

        await this._stockItemRepository.UpdateAsync(entity);

        var history = new StockUsageHistoryDto()
        {
            OrderItemId = orderItemId,
            WarehouseStockItemId = entity.Id,
            Quantity = deductQuantity,
            Revert = false,
            RevertTime = null
        };

        await this._stockUsageHistoryService.InsertAsync(history);

        return deductQuantity;
    }

    private async Task<StockItem[]> QueryStockItemsByCombinationIdAsync(int combinationId)
    {
        if (combinationId <= 0)
            throw new ArgumentNullException(nameof(combinationId));

        var db = await this._salesRepository.GetDbContextAsync();

        var now = this.Clock.Now;

        var query = from item in db.Set<StockItem>().AsNoTracking()
            join stock in db.Set<Stock>().AsNoTracking()
                on item.WarehouseStockId equals stock.Id
            select new { item, stock };

        query = query.Where(x => x.stock.Approved);
        //query = query.Where(x => x.stock.ExpirationTime > now);
        query = query
            .Where(x => x.item.CombinationId == combinationId)
            .Where(x => x.item.DeductQuantity < x.item.Quantity)
            .Where(x => !x.item.RuningOut);

        var warehouseStocks = await query
            .OrderBy(x => x.stock.CreationTime)
            .Select(x => x.item)
            .ToArrayAsync();

        return warehouseStocks;
    }

    public async Task DeductStockAsync(DeductStockInput dto)
    {
        if (string.IsNullOrWhiteSpace(dto.OrderItemId) || dto.CombinationId <= 0 || dto.Quantity <= 0)
            throw new ArgumentNullException(nameof(DeductStockAsync));

        using var dlock = await this.RedLockClient.RedLockFactory.CreateLockAsync(
            resource: $"{nameof(DeductStockAsync)}.{dto.CombinationId}",
            expiryTime: TimeSpan.FromSeconds(5));

        if (dlock.IsAcquired)
        {
            var warehouseStocks = await this.QueryStockItemsByCombinationIdAsync(dto.CombinationId);

            var totalStockToDeduct = dto.Quantity;

            foreach (var stockEntity in warehouseStocks)
            {
                var deductNumber = await this.DeductStockAsync(dto.OrderItemId, stockEntity.Id, totalStockToDeduct);
                
                //stock run out
                if (deductNumber <= 0)
                    break;
                
                //update current stock to deduct
                totalStockToDeduct -= deductNumber;
                
                //check if deduct is done
                if (totalStockToDeduct <= 0)
                    break;
            }

            if (totalStockToDeduct > 0)
            {
                throw new UserFriendlyException("stock is not enough");
            }
        }
        else
        {
            throw new FailToGetRedLockException(nameof(DeductStockAsync));
        }
    }
}