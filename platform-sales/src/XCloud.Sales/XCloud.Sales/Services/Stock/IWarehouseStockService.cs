using XCloud.Core.Application;
using XCloud.Core.Dto;
using XCloud.Core.Helper;
using XCloud.Database.EntityFrameworkCore.Extensions;
using XCloud.Platform.Shared.Dto;
using XCloud.Redis;
using XCloud.Sales.Clients.Platform;
using XCloud.Sales.Data;
using XCloud.Sales.Data.Domain.Catalog;
using XCloud.Sales.Data.Domain.Stock;
using XCloud.Sales.Services.Catalog;

namespace XCloud.Sales.Services.Stock;

public interface IWarehouseStockService : ISalesAppService
{
    Task DeleteUnApprovedStockAsync(string stockId);

    Task<WarehouseStockDto[]> AttachDataAsync(WarehouseStockDto[] data, AttachWarehouseStockDataInput dto);

    Task<WarehouseStockItemDto[]> AttachDataAsync(WarehouseStockItemDto[] data,
        AttachWarehouseStockItemDataInput dto);

    Task<PagedResponse<WarehouseStockDto>> QueryPagingAsync(QueryWarehouseStockInput dto);

    Task<PagedResponse<WarehouseStockItemDto>> QueryItemPagingAsync(QueryWarehouseStockItemInput dto);

    Task ApproveStockAsync(string stockId);

    Task InsertWarehouseStockAsync(WarehouseStockDto dto);

    Task DeductStockAsync(DeductStockInput dto);
}

public class WarehouseStockService : SalesAppService, IWarehouseStockService
{
    private readonly ISalesRepository<WarehouseStock> _salesRepository;
    private readonly PlatformInternalService _platformInternalService;
    private readonly IGoodsStockService _goodsStockService;

    public WarehouseStockService(ISalesRepository<WarehouseStock> salesRepository,
        PlatformInternalService platformInternalService,
        IGoodsStockService goodsStockService)
    {
        this._goodsStockService = goodsStockService;
        this._salesRepository = salesRepository;
        this._platformInternalService = platformInternalService;
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

    public async Task<WarehouseStockDto[]> AttachDataAsync(WarehouseStockDto[] data,
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
            var allItems = await db.Set<WarehouseStockItem>().AsNoTracking()
                .Where(x => ids.Contains(x.WarehouseStockId)).ToArrayAsync();
            foreach (var m in data)
            {
                var items = allItems.Where(x => x.WarehouseStockId == m.Id).ToArray();
                m.Items = items.Select(x => this.ObjectMapper.Map<WarehouseStockItem, WarehouseStockItemDto>(x))
                    .ToArray();
            }
        }

        return data;
    }

    public async Task<WarehouseStockItemDto[]> AttachDataAsync(WarehouseStockItemDto[] data,
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
            var allStocks = await db.Set<WarehouseStock>().AsNoTracking()
                .Where(x => ids.Contains(x.Id))
                .ToArrayAsync();
            foreach (var m in data)
            {
                var stock = allStocks.FirstOrDefault(x => x.Id == m.WarehouseStockId);
                if (stock == null)
                    continue;
                m.WarehouseStock = this.ObjectMapper.Map<WarehouseStock, WarehouseStockDto>(stock);
            }
        }

        return data;
    }

    private IQueryable<WarehouseStock> BuildStockQuery(DbContext db, QueryWarehouseStockInput dto)
    {
        var query = db.Set<WarehouseStock>().AsNoTracking();

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

    private IQueryable<WarehouseStockItem> BuildStockItemQuery(DbContext db, QueryWarehouseStockItemInput dto)
    {
        var query = db.Set<WarehouseStockItem>().AsNoTracking();

        if (dto.GoodsId != null)
            query = query.Where(x => x.GoodsId == dto.GoodsId.Value);

        if (dto.CombinationId != null)
            query = query.Where(x => x.CombinationId == dto.CombinationId.Value);

        if (dto.RunningOut != null)
            query = query.Where(x => x.RuningOut == dto.RunningOut.Value);

        return query;
    }

    public async Task<PagedResponse<WarehouseStockDto>> QueryPagingAsync(QueryWarehouseStockInput dto)
    {
        var db = await this._salesRepository.GetDbContextAsync();

        var query = this.BuildStockQuery(db, dto);

        var count = 0;
        if (!dto.SkipCalculateTotalCount)
            count = await query.CountAsync();

        var stocks = await query.OrderByDescending(x => x.CreationTime).PageBy(dto.AsAbpPagedRequestDto())
            .ToArrayAsync();

        var stockDtos = stocks.Select(x => this.ObjectMapper.Map<WarehouseStock, WarehouseStockDto>(x)).ToArray();

        return new PagedResponse<WarehouseStockDto>(stockDtos, dto, count);
    }

    public async Task<PagedResponse<WarehouseStockItemDto>> QueryItemPagingAsync(QueryWarehouseStockItemInput dto)
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

        WarehouseStockItemDto BuildResponse(WarehouseStockItem item, WarehouseStock stockOrNull)
        {
            var itemDto = this.ObjectMapper.Map<WarehouseStockItem, WarehouseStockItemDto>(item);

            if (stockOrNull != null)
                itemDto.WarehouseStock = this.ObjectMapper.Map<WarehouseStock, WarehouseStockDto>(stockOrNull);

            return itemDto;
        }

        var stocks = await query.OrderByDescending(x => x.stock.CreationTime).PageBy(dto.AsAbpPagedRequestDto())
            .ToArrayAsync();

        var stockItemDtos = stocks.Select(x => BuildResponse(x.item, x.stock)).ToArray();

        return new PagedResponse<WarehouseStockItemDto>(stockItemDtos, dto, count);
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

    private async Task CheckInsertWarehouseStockInputAsync(WarehouseStockDto dto)
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

    public async Task InsertWarehouseStockAsync(WarehouseStockDto dto)
    {
        await this.CheckInsertWarehouseStockInputAsync(dto);

        var db = await this._salesRepository.GetDbContextAsync();

        var entity = this.ObjectMapper.Map<WarehouseStockDto, WarehouseStock>(dto);
        entity.Id = this.GuidGenerator.CreateGuidString();
        entity.No = await this.GenerateNoAsync();
        entity.Approved = false;
        entity.ApprovedByUserId = string.Empty;
        entity.ApprovedTime = null;
        entity.CreationTime = this.Clock.Now;

        var items = dto.Items.Select(x => this.ObjectMapper.Map<WarehouseStockItemDto, WarehouseStockItem>(x))
            .ToArray();
        foreach (var m in items)
        {
            m.Id = this.GuidGenerator.CreateGuidString();
            m.WarehouseStockId = entity.Id;
            m.DeductQuantity = default;
            m.RuningOut = false;
            m.LastModificationTime = null;
        }

        db.Set<WarehouseStock>().Add(entity);
        db.Set<WarehouseStockItem>().AddRange(items);

        await db.SaveChangesAsync();
    }

    public async Task ApproveStockAsync(string stockId)
    {
        if (string.IsNullOrWhiteSpace(stockId))
            throw new ArgumentNullException(nameof(ApproveStockAsync));

        var db = await this._salesRepository.GetDbContextAsync();

        var entity = await db.Set<WarehouseStock>().FirstOrDefaultAsync(x => x.Id == stockId);
        if (entity == null)
            throw new EntityNotFoundException(nameof(stockId));

        if (entity.Approved)
            return;

        var items = await db.Set<WarehouseStockItem>().Where(x => x.WarehouseStockId == stockId).ToArrayAsync();
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

            var query = from item in db.Set<WarehouseStockItem>().AsTracking()
                join stock in db.Set<WarehouseStock>().AsTracking()
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

            if (!warehouseStocks.Any())
                throw new UserFriendlyException("stock runing out");

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
                //
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