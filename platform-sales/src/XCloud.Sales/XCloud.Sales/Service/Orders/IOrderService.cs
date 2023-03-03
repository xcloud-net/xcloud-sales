using XCloud.Core.Application;
using XCloud.Core.Dto;
using XCloud.Core.Helper;
using XCloud.Sales.Application;
using XCloud.Sales.Clients.Platform;
using XCloud.Sales.Data;
using XCloud.Sales.Data.Domain.Aftersale;
using XCloud.Sales.Data.Domain.Catalog;
using XCloud.Sales.Data.Domain.Orders;
using XCloud.Sales.Data.Domain.Stores;
using XCloud.Sales.Data.Domain.Users;
using XCloud.Sales.Service.AfterSale;
using XCloud.Sales.Service.Catalog;
using XCloud.Sales.Service.Users;

namespace XCloud.Sales.Service.Orders;

public interface IOrderService : ISalesAppService
{
    Task<OrderNoteDto[]> QueryOrderNotesAsync(QueryOrderNotesInput dto);

    Task<PlaceOrderCheckResponse[]> CheckGoodsStockStatusAsync(PlaceOrderCheckInput[] dtos);

    Task<PagedResponse<OrderDto>> QueryPagingAsync(QueryOrderInput dto);

    Task InsertOrderNoteAsync(OrderNote orderNote);

    Task<Order> QueryByIdAsync(string orderId);

    Task<OrderDto> QueryDetailByIdAsync(string orderId);

    Task<OrderDto[]> AttachDataV2Async(OrderDto[] orders, OrderAttachDataOption option);

    Task<OrderItemDto[]> AttachOrderItemDataAsync(OrderItemDto[] data, OrderItemAttachDataInput dto);

    Task UpdateStatusAsync(UpdateOrderInput dto);

    Task<int> QueryPendingCountAsync(QueryPendingCountInput dto);
}

public class OrderService : SalesAppService, IOrderService
{
    private readonly ISalesRepository<Order> _orderRepository;
    private readonly IGoodsService _goodsService;
    private readonly PlatformInternalService _platformInternalService;
    private readonly AfterSaleUtils _afterSaleUtils;
    private readonly OrderUtils _orderUtils;

    public OrderService(ISalesRepository<Order> orderRepository,
        IGoodsService goodsService,
        AfterSaleUtils afterSaleUtils,
        OrderUtils orderUtils,
        PlatformInternalService platformInternalService)
    {
        this._orderUtils = orderUtils;
        this._afterSaleUtils = afterSaleUtils;
        this._platformInternalService = platformInternalService;
        _orderRepository = orderRepository;
        this._goodsService = goodsService;
    }

    public async Task<int> QueryPendingCountAsync(QueryPendingCountInput dto)
    {
        if (dto == null)
            throw new ArgumentNullException(nameof(dto));

        var db = await this._orderRepository.GetDbContextAsync();

        var finishedStatus = this._orderUtils.DoneStatus();

        var query = db.Set<Order>().Where(x => !finishedStatus.Contains(x.OrderStatusId));

        if (dto.UserId != null && dto.UserId.Value > 0)
            query = query.Where(x => x.UserId == dto.UserId.Value);

        var count = await query.CountAsync();

        return count;
    }

    public virtual async Task UpdateStatusAsync(UpdateOrderInput dto)
    {
        if (dto == null)
            throw new ArgumentNullException(nameof(dto));

        var db = await this._orderRepository.GetDbContextAsync();

        var entity = await db.Set<Order>().IgnoreQueryFilters().FirstOrDefaultAsync(x => x.Id == dto.Id);

        if (entity == null)
            throw new EntityNotFoundException(nameof(UpdateStatusAsync));

        if (dto.HideForAdmin != null)
            entity.HideForAdmin = dto.HideForAdmin.Value;

        if (dto.IsDeleted != null)
            entity.IsDeleted = dto.IsDeleted.Value;

        entity.LastModificationTime = this.Clock.Now;

        await this._orderRepository.UpdateAsync(entity);
    }

    public virtual async Task<OrderNoteDto[]> QueryOrderNotesAsync(QueryOrderNotesInput dto)
    {
        if (string.IsNullOrWhiteSpace(dto.OrderId))
            throw new ArgumentNullException(nameof(dto.OrderId));

        var db = await this._orderRepository.GetDbContextAsync();

        var set = db.Set<OrderNote>().AsNoTracking();

        var query = set.Where(x => x.OrderId == dto.OrderId);

        if (dto.OnlyForUser)
            query = query.Where(x => x.DisplayToUser);

        var count = dto.MaxCount ?? 5000;

        var data = await query.OrderBy(x => x.CreationTime).Take(count).ToArrayAsync();

        var orderNotes = data.Select(x => this.ObjectMapper.Map<OrderNote, OrderNoteDto>(x)).ToArray();

        return orderNotes;
    }

    public virtual async Task<Order> QueryByIdAsync(string orderId)
    {
        if (string.IsNullOrWhiteSpace(orderId))
            return null;

        var order = await _orderRepository.QueryOneAsync(x => x.Id == orderId);

        return order;
    }

    public virtual async Task<OrderDto> QueryDetailByIdAsync(string orderId)
    {
        if (string.IsNullOrWhiteSpace(orderId))
            return null;

        var orderEntity = await _orderRepository.QueryOneAsync(x => x.Id == orderId);
        if (orderEntity == null)
            return null;

        var order = this.ObjectMapper.Map<Order, OrderDto>(orderEntity);

        await this.AttachOrderDetailDataAsync(new[] { order });

        return order;
    }

    public async Task<PlaceOrderCheckResponse[]> CheckGoodsStockStatusAsync(PlaceOrderCheckInput[] dtos)
    {
        if (ValidateHelper.IsEmptyCollection(dtos))
            return Array.Empty<PlaceOrderCheckResponse>();

        var combinationIds = dtos.Select(x => x.GoodsSpecCombinationId).Distinct().ToArray();

        var db = await this._orderRepository.GetDbContextAsync();

        var query = from combination in db.Set<GoodsSpecCombination>().AsNoTracking()
            join g in db.Set<Goods>().AsNoTracking()
                on combination.GoodsId equals g.Id into goodsGrouping
            from goods in goodsGrouping.DefaultIfEmpty()
            select new
            {
                combination,
                goods
            };

        var allCombinationAndGoods =
            await query.Where(x => combinationIds.Contains(x.combination.Id)).ToArrayAsync();

        var responses = new List<PlaceOrderCheckResponse>();

        foreach (var m in dtos)
        {
            var res = new PlaceOrderCheckResponse(m);

            var combinationAndGoods =
                allCombinationAndGoods.FirstOrDefault(x => x.combination.Id == m.GoodsSpecCombinationId);

            if (combinationAndGoods == null)
            {
                res.AddError("goods not exist");
            }
            else
            {
                if (combinationAndGoods.goods == null || !combinationAndGoods.goods.Published)
                    res.AddError("商品已经下架");

                if (m.Quantity > combinationAndGoods.combination.StockQuantity)
                    res.AddError("库存不足");

                if (!combinationAndGoods.combination.IsActive)
                    res.AddError("规格不可销售");
            }

            responses.Add(res);
        }

        return responses.ToArray();
    }

    public async Task<PlaceOrderCheckResponse[]> CheckGoodsStoreMappingAsync(PlaceOrderCheckInput[] dtos)
    {
        await Task.CompletedTask;
        //store mapping should bind with combination id
        throw new NotImplementedException();
    }

    private async Task<OrderDto[]> AttachOrderDetailDataAsync(OrderDto[] orders)
    {
        var data = await this.AttachDataV2Async(orders,
            new OrderAttachDataOption()
            {
                User = true,
                Store = true,
                OrderItems = true,
                AfterSales = true
            });

        var mallUsers = data.Select(x => x.User).WhereNotNull().ToArray();

        await this._platformInternalService.AttachSysUserAsync(mallUsers);

        var items = data.SelectMany(x => x.Items).ToArray();

        await this.AttachOrderItemDataAsync(items, new OrderItemAttachDataInput() { Goods = true });

        var allGoods = items.Select(x => x.Goods).WhereNotNull().ToArray();

        await this._goodsService.AttachDataAsync(allGoods, new AttachGoodsDataInput() { Images = true });

        return data;
    }

    public async Task<OrderDto[]> AttachDataV2Async(OrderDto[] orders, OrderAttachDataOption option)
    {
        if (!orders.Any())
            return Array.Empty<OrderDto>();

        var db = await this._orderRepository.GetDbContextAsync();

        if (option.AfterSales)
        {
            var afterSalesIds = orders.Select(x => x.AfterSalesId).WhereNotEmpty().Distinct().ToArray();
            if (afterSalesIds.Any())
            {
                var afterSales = await db.Set<AfterSales>().AsNoTracking().Where(x => afterSalesIds.Contains(x.Id))
                    .ToArrayAsync();
                foreach (var m in orders)
                {
                    var afterSalesEntity = afterSales.FirstOrDefault(x => x.Id == m.AfterSalesId);
                    if (afterSalesEntity == null)
                        continue;
                    m.AfterSales = this.ObjectMapper.Map<AfterSales, AfterSalesDto>(afterSalesEntity);
                }
            }
        }

        if (option.User)
        {
            var userIds = orders.Select(x => x.UserId).ToArray();

            var users = await db.Set<User>().AsNoTracking().IgnoreQueryFilters().Where(x => userIds.Contains(x.Id))
                .ToArrayAsync();

            foreach (var m in orders)
            {
                var u = users.FirstOrDefault(x => x.Id == m.UserId);
                if (u == null)
                    continue;

                m.User = this.ObjectMapper.Map<User, StoreUserDto>(u);
            }
        }

        if (option.Store)
        {
            var storeIds = orders.Select(x => x.StoreId).WhereNotEmpty().Distinct().ToArray();

            var stores = await db.Set<Store>().AsNoTracking().Where(x => storeIds.Contains(x.Id)).ToArrayAsync();

            foreach (var m in orders)
            {
                var store = stores.FirstOrDefault(x => x.Id == m.StoreId);

                m.StoreName = store?.StoreName;
            }
        }

        if (option.OrderItems)
        {
            var query = db.Set<OrderItem>().AsNoTracking();

            var orderIds = orders.Ids().ToArray();

            query = query.Where(x => orderIds.Contains(x.OrderId));

            var items = await query.ToArrayAsync();

            foreach (var m in orders)
            {
                var itemArray = items.Where(x => x.OrderId == m.Id).ToArray();

                m.Items = itemArray.Select(x => this.ObjectMapper.Map<OrderItem, OrderItemDto>(x)).ToArray();
            }
        }

        if (option.Activity)
        {
            throw new NotImplementedException();
        }

        if (option.Coupon)
        {
            throw new NotImplementedException();
        }

        if (option.Address)
        {
            throw new NotImplementedException();
        }

        return orders;
    }

    public async Task<OrderItemDto[]> AttachOrderItemDataAsync(OrderItemDto[] data, OrderItemAttachDataInput dto)
    {
        if (!data.Any())
            return Array.Empty<OrderItemDto>();

        var db = await this._orderRepository.GetDbContextAsync();

        if (dto.Goods)
        {
            var query = from item in db.Set<OrderItem>().AsNoTracking()
                join g in db.Set<Goods>().IgnoreQueryFilters().AsNoTracking()
                    on item.GoodsId equals g.Id into goodsGrouping
                from goods in goodsGrouping.DefaultIfEmpty()
                join c in db.Set<GoodsSpecCombination>().IgnoreQueryFilters().AsNoTracking()
                    on item.GoodsSpecCombinationId equals c.Id into cGrouping
                from combination in cGrouping.DefaultIfEmpty()
                select new { item, goods, combination };

            var itemIds = data.Ids().ToArray();

            query = query.Where(x => itemIds.Contains(x.item.Id));

            var items = await query.ToArrayAsync();

            foreach (var m in data)
            {
                var grouped = items.Where(x => x.item.Id == m.Id).FirstOrDefault();
                if (grouped == null)
                    continue;

                if (grouped.goods != null)
                    m.Goods = this.ObjectMapper.Map<Goods, GoodsDto>(grouped.goods);

                if (grouped.combination != null)
                    m.GoodsSpecCombination =
                        this.ObjectMapper.Map<GoodsSpecCombination, GoodsSpecCombinationDto>(grouped.combination);
            }
        }

        return data;
    }

    private async Task<IQueryable<string>> BuildOrderIdsQueryAsync(QueryOrderInput dto)
    {
        var db = await this._orderRepository.GetDbContextAsync();

        var query = from order in db.Set<Order>().IgnoreQueryFilters().AsNoTracking()
            select new
            {
                order
            };

        if (!string.IsNullOrWhiteSpace(dto.Sn))
            query = query.Where(x => x.order.OrderSn == dto.Sn);
        if (dto.AffiliateId != null)
            query = query.Where(x => x.order.AffiliateId == dto.AffiliateId);
        if (dto.UserId != null)
            query = query.Where(o => o.order.UserId == dto.UserId.Value);
        if (dto.StartTime.HasValue)
            query = query.Where(o => dto.StartTime.Value <= o.order.CreationTime);
        if (dto.EndTime.HasValue)
            query = query.Where(o => dto.EndTime.Value >= o.order.CreationTime);

        if (ValidateHelper.IsNotEmptyCollection(dto.Status))
            query = query.Where(o => dto.Status.Contains(o.order.OrderStatusId));

        if (dto.PaymentStatus != null)
            query = query.Where(x => x.order.PaymentStatusId == dto.PaymentStatus.Value);

        if (dto.ShippingStatus != null)
            query = query.Where(x => x.order.ShippingStatusId == dto.ShippingStatus.Value);

        if (dto.IsDeleted != null)
            query = query.Where(o => o.order.IsDeleted == dto.IsDeleted.Value);

        if (dto.HideForAdmin != null)
            query = query.Where(x => x.order.HideForAdmin == dto.HideForAdmin.Value);

        if (!string.IsNullOrWhiteSpace(dto.StoreId))
            query = query.Where(x => x.order.StoreId == dto.StoreId);

        //after sales relative query
        var afterSaleJoinQuery = from order in query
            join aftersale in db.Set<AfterSales>().IgnoreQueryFilters().AsNoTracking()
                on order.order.AfterSalesId equals aftersale.Id into aftersaleGrouping
            from aftersaleOrNull in aftersaleGrouping.DefaultIfEmpty()
            select new { order, aftersaleOrNull };
        //ref to join query
        var originAfterSaleJoinQuery = afterSaleJoinQuery;

        if (dto.IsAfterSales != null)
            afterSaleJoinQuery =
                afterSaleJoinQuery.Where(x => x.order.order.IsAftersales == dto.IsAfterSales.Value);

        if (dto.IsAfterSalesPending ?? false)
        {
            var pendingStatus = this._afterSaleUtils.PendingStatus();

            afterSaleJoinQuery =
                afterSaleJoinQuery.Where(x => pendingStatus.Contains(x.aftersaleOrNull.AfterSalesStatusId));
        }

        if (ValidateHelper.IsNotEmptyCollection(dto.AfterSalesStatus))
        {
            afterSaleJoinQuery =
                afterSaleJoinQuery.Where(x => dto.AfterSalesStatus.Contains(x.aftersaleOrNull.AfterSalesStatusId));
        }

        if (!object.ReferenceEquals(originAfterSaleJoinQuery, afterSaleJoinQuery))
            query = afterSaleJoinQuery.Select(x => x.order);

        return query.Select(x => x.order.Id).Distinct();
    }

    public async Task<PagedResponse<OrderDto>> QueryPagingAsync(QueryOrderInput dto)
    {
        var db = await this._orderRepository.GetDbContextAsync();

        var idsQuery = await this.BuildOrderIdsQueryAsync(dto);

        var query = db.Set<Order>().IgnoreQueryFilters().AsNoTracking();

        query = query.Where(x => idsQuery.Contains(x.Id));

        var count = 0;
        if (!dto.SkipCalculateTotalCount)
            count = await query.CountAsync();

        if (dto.SortForAdmin ?? false)
        {
            query = query
                .OrderBy(x => x.HideForAdmin)
                .ThenByDescending(x => x.CreationTime);
        }
        else
        {
            query = query
                .OrderBy(x => x.IsDeleted)
                .ThenByDescending(x => x.CreationTime);
        }

        var items = await query.PageBy(dto.ToAbpPagedRequest()).ToArrayAsync();

        var orderDtos = items.Select(x => this.ObjectMapper.Map<Order, OrderDto>(x)).ToArray();

        orderDtos = await this.AttachOrderDetailDataAsync(orderDtos);

        return new PagedResponse<OrderDto>(orderDtos, dto, count);
    }

    public async Task InsertOrderNoteAsync(OrderNote orderNote)
    {
        if (orderNote == null)
            throw new ArgumentNullException(nameof(orderNote));

        if (string.IsNullOrWhiteSpace(orderNote.OrderId))
            throw new ArgumentNullException(nameof(orderNote.OrderId));

        var db = await this._orderRepository.GetDbContextAsync();

        orderNote.CreationTime = this.Clock.Now;

        db.Set<OrderNote>().Add(orderNote);

        await db.SaveChangesAsync();
    }
}