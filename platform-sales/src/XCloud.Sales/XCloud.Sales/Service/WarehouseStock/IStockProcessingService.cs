using XCloud.Database.EntityFrameworkCore.Extensions;
using XCloud.Redis;
using XCloud.Sales.Application;
using XCloud.Sales.Clients.Platform;
using XCloud.Sales.Data;
using XCloud.Sales.Data.Domain.WarehouseStock;
using XCloud.Sales.Service.Catalog;

namespace XCloud.Sales.Service.WarehouseStock;

public interface IStockProcessingService : ISalesAppService
{
    Task UpdateGoodsStockFromWarehouseStockAsync(int combinationId);

    Task DeductStockAsync(DeductStockInput dto);
}

public class StockProcessingService : SalesAppService, IStockProcessingService
{
    private readonly ISalesRepository<Stock> _salesRepository;
    private readonly IStockUsageHistoryService _stockUsageHistoryService;
    private readonly PlatformInternalService _platformInternalService;
    private readonly IGoodsStockService _goodsStockService;
    private readonly IStockService _stockService;

    public StockProcessingService(ISalesRepository<Stock> salesRepository,
        PlatformInternalService platformInternalService,
        IStockUsageHistoryService stockUsageHistoryService,
        IGoodsStockService goodsStockService, IStockService stockService)
    {
        _stockUsageHistoryService = stockUsageHistoryService;
        _goodsStockService = goodsStockService;
        _stockService = stockService;
        this._salesRepository = salesRepository;
        this._platformInternalService = platformInternalService;
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

    public async Task DeductStockAsync(DeductStockInput dto)
    {
        if (dto.CombinationId <= 0 || dto.Quantity <= 0)
            throw new ArgumentNullException(nameof(DeductStockAsync));

        using var dlock = await this.RedLockClient.RedLockFactory.CreateLockAsync(
            resource: $"{nameof(DeductStockAsync)}.{dto.CombinationId}",
            expiryTime: TimeSpan.FromSeconds(5));

        if (dlock.IsAcquired)
        {
            var db = await this._salesRepository.GetDbContextAsync();

            var now = this.Clock.Now;

            var query = from item in db.Set<StockItem>().AsTracking()
                join stock in db.Set<Stock>().AsTracking()
                    on item.WarehouseStockId equals stock.Id
                select new { item, stock };

            query = query.Where(x => x.stock.Approved);
            //query = query.Where(x => x.stock.ExpirationTime > now);
            query = query
                .Where(x => x.item.CombinationId == dto.CombinationId)
                .Where(x => x.item.DeductQuantity < x.item.Quantity)
                .Where(x => !x.item.RuningOut);

            var warehouseStocks = await query
                .OrderBy(x => x.stock.CreationTime)
                .ToArrayAsync();

            var totalStockToDeduct = dto.Quantity;

            foreach (var m in warehouseStocks)
            {
                var stockEntity = m.item;
                var quantityAvailable = stockEntity.Quantity - stockEntity.DeductQuantity;
                var quantityToDeduct = Math.Min(quantityAvailable, totalStockToDeduct);
                //
                stockEntity.DeductQuantity += quantityToDeduct;
                if (stockEntity.DeductQuantity >= stockEntity.Quantity)
                {
                    stockEntity.RuningOut = true;
                }

                stockEntity.LastModificationTime = now;

                //check deduct is done?
                totalStockToDeduct -= quantityToDeduct;
                if (totalStockToDeduct <= 0)
                {
                    //deduct finished
                    break;
                }
            }

            if (totalStockToDeduct > 0)
            {
                throw new UserFriendlyException("stock is not enough");
            }

            await db.TrySaveChangesAsync();
        }
        else
        {
            throw new FailToGetRedLockException(nameof(DeductStockAsync));
        }
    }
}