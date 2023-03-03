using XCloud.Core.Application;
using XCloud.Core.Dto;
using XCloud.Core.Helper;
using XCloud.Database.EntityFrameworkCore.Extensions;
using XCloud.Platform.Shared.Dto;
using XCloud.Sales.Application;
using XCloud.Sales.Clients.Platform;
using XCloud.Sales.Data;
using XCloud.Sales.Data.Domain.Catalog;
using XCloud.Sales.Data.Domain.WarehouseStock;
using XCloud.Sales.Service.Catalog;

namespace XCloud.Sales.Service.WarehouseStock;

public interface IStockService : ISalesAppService
{
    Task<int> QueryGoodsCombinationWarehouseStockAsync(int combinationId);

    Task DeleteUnApprovedStockAsync(string stockId);

    Task<StockDto[]> AttachDataAsync(StockDto[] data, AttachWarehouseStockDataInput dto);

    Task<StockItemDto[]> AttachDataAsync(StockItemDto[] data,
        AttachWarehouseStockItemDataInput dto);

    Task<PagedResponse<StockDto>> QueryPagingAsync(QueryWarehouseStockInput dto);

    Task<PagedResponse<StockItemDto>> QueryItemPagingAsync(QueryWarehouseStockItemInput dto);

    Task ApproveStockAsync(string stockId);

    Task InsertWarehouseStockAsync(StockDto dto);
}

public class StockService : SalesAppService, IStockService
{
    private readonly ISalesRepository<Stock> _salesRepository;
    private readonly IStockUsageHistoryService _stockUsageHistoryService;
    private readonly PlatformInternalService _platformInternalService;
    private readonly IGoodsStockService _goodsStockService;

    public StockService(ISalesRepository<Stock> salesRepository,
        PlatformInternalService platformInternalService,
        IStockUsageHistoryService stockUsageHistoryService,
        IGoodsStockService goodsStockService)
    {
        _stockUsageHistoryService = stockUsageHistoryService;
        _goodsStockService = goodsStockService;
        this._salesRepository = salesRepository;
        this._platformInternalService = platformInternalService;
    }

    public async Task<int> QueryGoodsCombinationWarehouseStockAsync(int combinationId)
    {
        if (combinationId <= 0)
            throw new ArgumentNullException(nameof(combinationId));

        var now = this.Clock.Now;

        var db = await this._salesRepository.GetDbContextAsync();

        var query = from item in db.Set<StockItem>().AsNoTracking()
            join stock in db.Set<Stock>().AsNoTracking()
                on item.WarehouseStockId equals stock.Id
            select new { item, stock };
        query = query.Where(x => x.stock.Approved);
        query = query.Where(x => x.item.CombinationId == combinationId);
        query = query.Where(x => !x.item.RuningOut);

        var count = await query.SumAsync(x => x.item.Quantity - x.item.DeductQuantity);
        return count;
    }

    public async Task DeleteUnApprovedStockAsync(string stockId)
    {
        if (string.IsNullOrWhiteSpace(stockId))
            throw new ArgumentNullException(nameof(stockId));

        var entity = await this._salesRepository.QueryOneAsync(x => x.Id == stockId);
        if (entity == null)
            throw new EntityNotFoundException(nameof(DeleteUnApprovedStockAsync));

        if (entity.Approved)
            throw new UserFriendlyException("this stock is approved,can't be deleted");

        await this._salesRepository.DeleteAsync(entity);
    }

    public async Task<StockDto[]> AttachDataAsync(StockDto[] data,
        AttachWarehouseStockDataInput dto)
    {
        if (!data.Any())
            return data;

        var ids = data.Ids().ToArray();

        var db = await this._salesRepository.GetDbContextAsync();

        if (dto.Warehouse)
        {
            var warehouseIds = data
                .Where(x => !string.IsNullOrWhiteSpace(x.WarehouseId))
                .Select(x => x.WarehouseId).Distinct().ToArray();
            if (warehouseIds.Any())
            {
                var warehouses = await db.Set<Warehouse>().Where(x => warehouseIds.Contains(x.Id)).ToArrayAsync();
                foreach (var m in data)
                {
                    var warehouse = warehouses.FirstOrDefault(x => x.Id == m.WarehouseId);
                    if (warehouse == null)
                        continue;
                    m.Warehouse = this.ObjectMapper.Map<Warehouse, WarehouseDto>(warehouse);
                }
            }
        }

        if (dto.Supplier)
        {
            var supplierIds = data
                .Where(x => !string.IsNullOrWhiteSpace(x.SupplierId))
                .Select(x => x.SupplierId).Distinct().ToArray();
            if (supplierIds.Any())
            {
                var suppliers = await db.Set<Supplier>().Where(x => supplierIds.Contains(x.Id)).ToArrayAsync();
                foreach (var m in data)
                {
                    var supplier = suppliers.FirstOrDefault(x => x.Id == m.WarehouseId);
                    if (supplier == null)
                        continue;
                    m.Supplier = this.ObjectMapper.Map<Supplier, SupplierDto>(supplier);
                }
            }
        }

        if (dto.Items)
        {
            var allItems = await db.Set<StockItem>().AsNoTracking()
                .Where(x => ids.Contains(x.WarehouseStockId)).ToArrayAsync();
            foreach (var m in data)
            {
                var items = allItems.Where(x => x.WarehouseStockId == m.Id).ToArray();
                m.Items = items.Select(x => this.ObjectMapper.Map<StockItem, StockItemDto>(x))
                    .ToArray();
            }
        }

        return data;
    }

    public async Task<StockItemDto[]> AttachDataAsync(StockItemDto[] data,
        AttachWarehouseStockItemDataInput dto)
    {
        if (!data.Any())
            return data;

        var db = await this._salesRepository.GetDbContextAsync();

        if (dto.Goods)
        {
            var combinationIds = data.Select(x => x.CombinationId).ToArray();

            var query = from combination in db.Set<GoodsSpecCombination>().AsNoTracking()
                join g in db.Set<Goods>().AsNoTracking()
                    on combination.GoodsId equals g.Id into goodsGrouping
                from goods in goodsGrouping.DefaultIfEmpty()
                select new { combination, goods };

            var goodsList = await query.Where(x => combinationIds.Contains(x.combination.Id)).ToArrayAsync();

            foreach (var m in data)
            {
                var goodsItem = goodsList.FirstOrDefault(x => x.combination.Id == m.CombinationId);
                if (goodsItem == null)
                    continue;

                m.Combination =
                    this.ObjectMapper.Map<GoodsSpecCombination, GoodsSpecCombinationDto>(goodsItem.combination);
                if (goodsItem.goods != null)
                    m.Goods = this.ObjectMapper.Map<Goods, GoodsDto>(goodsItem.goods);
            }
        }

        if (dto.WarehouseStock)
        {
            var ids = data.Select(x => x.WarehouseStockId).Distinct().ToArray();
            var allStocks = await db.Set<Stock>().AsNoTracking()
                .Where(x => ids.Contains(x.Id))
                .ToArrayAsync();
            foreach (var m in data)
            {
                var stock = allStocks.FirstOrDefault(x => x.Id == m.WarehouseStockId);
                if (stock == null)
                    continue;
                m.Stock = this.ObjectMapper.Map<Stock, StockDto>(stock);
            }
        }

        return data;
    }

    private IQueryable<Stock> BuildStockQuery(DbContext db, QueryWarehouseStockInput dto)
    {
        var query = db.Set<Stock>().AsNoTracking();

        if (!string.IsNullOrWhiteSpace(dto.SeiralNo))
            query = query.Where(x => x.No == dto.SeiralNo);

        if (!string.IsNullOrWhiteSpace(dto.SupplierId))
            query = query.Where(x => x.SupplierId == dto.SupplierId);

        if (!string.IsNullOrWhiteSpace(dto.WarehouseId))
            query = query.Where(x => x.WarehouseId == dto.WarehouseId);

        if (dto.Approved != null)
            query = query.Where(x => x.Approved == dto.Approved.Value);

        if (dto.StartTime != null)
            query = query.Where(x => x.CreationTime >= dto.StartTime.Value);

        if (dto.EndTime != null)
            query = query.Where(x => x.CreationTime <= dto.EndTime.Value);

        return query;
    }

    private IQueryable<StockItem> BuildStockItemQuery(DbContext db, QueryWarehouseStockItemInput dto)
    {
        var query = db.Set<StockItem>().AsNoTracking();

        if (dto.GoodsId != null)
            query = query.Where(x => x.GoodsId == dto.GoodsId.Value);

        if (dto.CombinationId != null)
            query = query.Where(x => x.CombinationId == dto.CombinationId.Value);

        if (dto.RunningOut != null)
            query = query.Where(x => x.RuningOut == dto.RunningOut.Value);

        return query;
    }

    public async Task<PagedResponse<StockDto>> QueryPagingAsync(QueryWarehouseStockInput dto)
    {
        var db = await this._salesRepository.GetDbContextAsync();

        var query = this.BuildStockQuery(db, dto);

        var count = 0;
        if (!dto.SkipCalculateTotalCount)
            count = await query.CountAsync();

        var stocks = await query.OrderByDescending(x => x.CreationTime).PageBy(dto.ToAbpPagedRequest())
            .ToArrayAsync();

        var stockDtos = stocks.Select(x => this.ObjectMapper.Map<Stock, StockDto>(x)).ToArray();

        return new PagedResponse<StockDto>(stockDtos, dto, count);
    }

    public async Task<PagedResponse<StockItemDto>> QueryItemPagingAsync(QueryWarehouseStockItemInput dto)
    {
        var db = await this._salesRepository.GetDbContextAsync();

        var stockQuery = this.BuildStockQuery(db, dto);
        var itemQuery = this.BuildStockItemQuery(db, dto);

        var query = from item in itemQuery
            join s in stockQuery
                on item.WarehouseStockId equals s.Id into stockGroup
            from stock in stockGroup.DefaultIfEmpty()
            select new { item, stock };

        var count = 0;
        if (!dto.SkipCalculateTotalCount)
            count = await query.CountAsync();

        StockItemDto BuildResponse(StockItem item, Stock stockOrNull)
        {
            var itemDto = this.ObjectMapper.Map<StockItem, StockItemDto>(item);

            if (stockOrNull != null)
                itemDto.Stock = this.ObjectMapper.Map<Stock, StockDto>(stockOrNull);

            return itemDto;
        }

        var stocks = await query.OrderByDescending(x => x.stock.CreationTime).PageBy(dto.ToAbpPagedRequest())
            .ToArrayAsync();

        var stockItemDtos = stocks.Select(x => BuildResponse(x.item, x.stock)).ToArray();

        return new PagedResponse<StockItemDto>(stockItemDtos, dto, count);
    }

    private string MonthString() => this.Clock.Now.ToString("yyyyMM");

    private async Task<string> GenerateNoAsync()
    {
        var month = this.MonthString();
        var response =
            await this._platformInternalService.GenerateSerialNoAsync(
                new CreateNoByCategoryDto($"warehouse-stock-no@{month}"));
        response.ThrowIfErrorOccured();
        var serialNo = response.Data;

        return $"{month}{serialNo.ToString().PadLeft(9, '0')}";
    }

    private async Task CheckInsertWarehouseStockInputAsync(StockDto dto)
    {
        if (dto == null)
            throw new ArgumentNullException(nameof(dto));

        if (ValidateHelper.IsEmptyCollection(dto.Items))
            throw new ArgumentNullException(nameof(dto.Items));

        foreach (var m in dto.Items)
        {
            if (m.GoodsId <= 0 || m.CombinationId <= 0)
                throw new UserFriendlyException("goods information is lost");

            if (m.Quantity <= 0)
                throw new UserFriendlyException(nameof(m.Quantity));

            if (m.Price < 0)
                throw new UserFriendlyException(nameof(m.Price));
        }

        await Task.CompletedTask;
    }

    public async Task InsertWarehouseStockAsync(StockDto dto)
    {
        await this.CheckInsertWarehouseStockInputAsync(dto);

        var db = await this._salesRepository.GetDbContextAsync();

        var entity = this.ObjectMapper.Map<StockDto, Stock>(dto);
        entity.Id = this.GuidGenerator.CreateGuidString();
        entity.No = await this.GenerateNoAsync();
        entity.Approved = false;
        entity.ApprovedByUserId = string.Empty;
        entity.ApprovedTime = null;
        entity.CreationTime = this.Clock.Now;

        var items = dto.Items.Select(x => this.ObjectMapper.Map<StockItemDto, StockItem>(x))
            .ToArray();
        foreach (var m in items)
        {
            m.Id = this.GuidGenerator.CreateGuidString();
            m.WarehouseStockId = entity.Id;
            m.DeductQuantity = default;
            m.RuningOut = false;
            m.LastModificationTime = null;
        }

        db.Set<Stock>().Add(entity);
        db.Set<StockItem>().AddRange(items);

        await db.SaveChangesAsync();
    }

    public async Task ApproveStockAsync(string stockId)
    {
        if (string.IsNullOrWhiteSpace(stockId))
            throw new ArgumentNullException(nameof(ApproveStockAsync));

        var db = await this._salesRepository.GetDbContextAsync();

        var entity = await db.Set<Stock>().FirstOrDefaultAsync(x => x.Id == stockId);
        if (entity == null)
            throw new EntityNotFoundException(nameof(stockId));

        if (entity.Approved)
            return;

        var items = await db.Set<StockItem>().Where(x => x.WarehouseStockId == stockId).ToArrayAsync();
        if (!items.Any())
            throw new EntityNotFoundException("stock items not found");

        var combinationIds = items.Select(x => x.CombinationId).Distinct().ToArray();

        var combinations = await db.Set<GoodsSpecCombination>().IgnoreQueryFilters()
            .Where(x => combinationIds.Contains(x.Id))
            .ToArrayAsync();

        foreach (var m in items)
        {
            var combination = combinations.FirstOrDefault(x => x.Id == m.CombinationId);
            if (combination == null)
                throw new UserFriendlyException("goods spec combination not exist");

            var totalPrice = (combination.StockQuantity * combination.CostPrice + m.Quantity * m.Price);
            var totalAmount = combination.StockQuantity + m.Quantity;

            if (totalAmount == 0)
            {
                combination.CostPrice = default;
            }
            else
            {
                //calculate new cost price
                combination.CostPrice = totalPrice / totalAmount;
            }

            //update stock
            combination.StockQuantity = totalAmount;
        }

        entity.Approved = true;
        entity.ApprovedTime = this.Clock.Now;

        await db.TrySaveChangesAsync();
    }
}