using XCloud.Core.Dto;
using XCloud.Sales.Application;
using XCloud.Sales.Data;
using XCloud.Sales.Data.Domain.Catalog;
using XCloud.Sales.Data.Domain.Orders;
using XCloud.Sales.Data.Domain.Shipping;
using XCloud.Sales.Data.Domain.Users;

namespace XCloud.Sales.Service.Report;

public interface IOrderReportService : ISalesAppService
{
    Task<TopCustomersList[]> TopCustomersAsync(QueryTopCustomerInput dto);

    Task<TopSkuList[]> TopSkuListsAsync(QueryTopSkuListInput dto);

    Task<TopSellersList[]> TopSellersAsync(QueryTopSellerInput dto);

    Task<TopBrandList[]> TopBrandListsAsync(QueryTopBrandListInput dto);

    Task<TopCategoryList[]> TopCategoryListsAsync(QueryTopCategoryListInput dto);

    Task<OrderSumByDateResponse[]> OrderSumByDateAsync(OrderSumByDateInput dto);
}

public class OrderReportService : SalesAppService, IOrderReportService
{
    private readonly ISalesRepository<Order> _orderRepository;

    public OrderReportService(ISalesRepository<Order> orderRepository)
    {
        _orderRepository = orderRepository;
    }

    public async Task<OrderSumByDateResponse[]> OrderSumByDateAsync(OrderSumByDateInput dto)
    {
        if (dto == null)
            throw new ArgumentNullException(nameof(dto));
        if (dto.StartTime == null || dto.EndTime == null)
            throw new UserFriendlyException("time range");
        if (dto.EndTime.Value <= dto.StartTime.Value)
            throw new UserFriendlyException("time range error");
        if (dto.EndTime.Value - dto.EndTime.Value > TimeSpan.FromDays(100))
            throw new UserFriendlyException("time range error");

        var db = await this._orderRepository.GetDbContextAsync();

        var query = db.Set<Order>().AsNoTracking();

        if (dto.StartTime != null)
            query = query.Where(x => x.CreationTime >= dto.StartTime.Value);

        if (dto.EndTime != null)
            query = query.Where(x => x.CreationTime < dto.EndTime.Value);

        var grouped = query.GroupBy(x => x.CreationTime.AddHours(8).Date).Select(x =>
            new { x.Key, count = x.Count(), total = x.Sum(d => d.OrderTotal) });
        var data = await grouped.Take(100).ToArrayAsync();

        var response = data.OrderBy(x => x.Key).Select(x => new OrderSumByDateResponse()
        {
            Date = x.Key,
            Total = x.count,
            Amount = x.total,
        }).ToArray();

        return response;
    }

    public async Task<TopSkuList[]> TopSkuListsAsync(QueryTopSkuListInput dto)
    {
        var db = await this._orderRepository.GetDbContextAsync();

        var query = from orderItem in db.Set<OrderItem>().AsNoTracking()
            join order in db.Set<Order>().AsNoTracking()
                on orderItem.OrderId equals order.Id
            select new { orderItem, order };

        if (dto.StartTime != null)
            query = query.Where(x => x.order.CreationTime >= dto.StartTime.Value);

        if (dto.EndTime != null)
            query = query.Where(x => x.order.CreationTime < dto.EndTime.Value);

        var status = (int)OrderStatus.Complete;
        query = query.Where(x => x.order.OrderStatusId == status);

        var groupedQuery = query.GroupBy(x => x.orderItem.GoodsSpecCombinationId).Select(x => new
        {
            x.Key,
            totalAmount = x.Sum(d => d.orderItem.Price),
            totalQuantity = x.Sum(d => d.orderItem.Quantity)
        });

        groupedQuery = groupedQuery.OrderByDescending(x => x.totalAmount).ThenByDescending(x => x.totalQuantity);

        var count = dto.MaxCount ?? 20;

        var data = await groupedQuery.Take(count).ToArrayAsync();

        var topskus = data.Select(x => new TopSkuList()
        {
            SkuId = x.Key,
            Name = String.Empty,
            TotalPrice = x.totalAmount,
            TotalQuantity = x.totalQuantity
        }).ToArray();

        if (topskus.Any())
        {
            var ids = topskus.Select(x => x.SkuId).ToArray();

            var q = from sku in db.Set<GoodsSpecCombination>().AsNoTracking()
                join goods in db.Set<Goods>().AsNoTracking()
                    on sku.GoodsId equals goods.Id
                select new { sku, goods };

            var skus = await q.Where(x => ids.Contains(x.sku.Id)).ToArrayAsync();

            foreach (var m in topskus)
            {
                var b = skus.FirstOrDefault(x => x.sku.Id == m.SkuId);
                if (b == null)
                    continue;
                m.Name = $"{b.goods.Name}-{b.sku.Name}";
            }
        }

        return topskus;
    }

    public async Task<TopBrandList[]> TopBrandListsAsync(QueryTopBrandListInput dto)
    {
        var db = await this._orderRepository.GetDbContextAsync();

        var query = from orderItem in db.Set<OrderItem>().AsNoTracking()
            join order in db.Set<Order>().AsNoTracking()
                on orderItem.OrderId equals order.Id
            join goods in db.Set<Goods>().AsNoTracking()
                on orderItem.GoodsId equals goods.Id
            select new { orderItem, goods, order };

        query = query.Where(x => x.goods.BrandId > 0);

        if (dto.StartTime != null)
            query = query.Where(x => x.order.CreationTime >= dto.StartTime.Value);

        if (dto.EndTime != null)
            query = query.Where(x => x.order.CreationTime < dto.EndTime.Value);

        var status = (int)OrderStatus.Complete;
        query = query.Where(x => x.order.OrderStatusId == status);

        var groupedQuery = query.GroupBy(x => x.goods.BrandId).Select(x => new
        {
            x.Key,
            totalAmount = x.Sum(d => d.orderItem.Price),
            totalQuantity = x.Sum(d => d.orderItem.Quantity)
        });

        groupedQuery = groupedQuery.OrderByDescending(x => x.totalAmount).ThenByDescending(x => x.totalQuantity);

        var count = dto.MaxCount ?? 20;

        var data = await groupedQuery.Take(count).ToArrayAsync();

        var topbrands = data.Select(x => new TopBrandList()
        {
            BrandId = x.Key,
            BrandName = String.Empty,
            TotalPrice = x.totalAmount,
            TotalQuantity = x.totalQuantity
        }).ToArray();

        if (topbrands.Any())
        {
            var ids = topbrands.Select(x => x.BrandId).ToArray();
            var brands = await db.Set<Brand>().AsNoTracking().Where(x => ids.Contains(x.Id)).ToArrayAsync();

            foreach (var m in topbrands)
            {
                var b = brands.FirstOrDefault(x => x.Id == m.BrandId);
                if (b == null)
                    continue;
                m.BrandName = b.Name;
            }
        }

        return topbrands;
    }

    public async Task<TopCategoryList[]> TopCategoryListsAsync(QueryTopCategoryListInput dto)
    {
        var db = await this._orderRepository.GetDbContextAsync();

        var query = from orderItem in db.Set<OrderItem>().AsNoTracking()
            join order in db.Set<Order>().AsNoTracking()
                on orderItem.OrderId equals order.Id
            join goods in db.Set<Goods>().AsNoTracking()
                on orderItem.GoodsId equals goods.Id
            join category in db.Set<Category>().AsNoTracking()
                on goods.CategoryId equals category.Id
            select new { orderItem, goods, category, order };

        query = query.Where(x => x.goods.CategoryId > 0);

        if (dto.StartTime != null)
            query = query.Where(x => x.order.CreationTime >= dto.StartTime.Value);

        if (dto.EndTime != null)
            query = query.Where(x => x.order.CreationTime < dto.EndTime.Value);

        var status = (int)OrderStatus.Complete;
        query = query.Where(x => x.order.OrderStatusId == status);

        var groupedQuery = query.GroupBy(x => new { x.category.RootId, x.category.Id }).Select(x => new
        {
            x.Key.RootId,
            x.Key.Id,
            totalAmount = x.Sum(d => d.orderItem.Price),
            totalQuantity = x.Sum(d => d.orderItem.Quantity)
        });

        groupedQuery = groupedQuery.OrderByDescending(x => x.totalAmount).ThenByDescending(x => x.totalQuantity);

        var count = dto.MaxCount ?? 20;

        var data = await groupedQuery.Take(count).ToArrayAsync();

        var topCategories = data.Select(x => new TopCategoryList()
        {
            RootCategoryId = x.RootId,
            RootCategoryName = string.Empty,
            CategoryId = x.Id,
            CategoryName = String.Empty,
            TotalPrice = x.totalAmount,
            TotalQuantity = x.totalQuantity
        }).ToArray();

        if (topCategories.Any())
        {
            var ids = topCategories.SelectMany(x => new[] { x.RootCategoryId, x.CategoryId }).Distinct().ToArray();
            var categories = await db.Set<Category>().AsNoTracking().Where(x => ids.Contains(x.Id)).ToArrayAsync();

            foreach (var m in topCategories)
            {
                var root = categories.FirstOrDefault(x => x.Id == m.RootCategoryId);
                var cat = categories.FirstOrDefault(x => x.Id == m.CategoryId);

                if (root != null)
                    m.RootCategoryName = root.Name;

                if (cat != null)
                    m.CategoryName = cat.Name;
            }
        }

        return topCategories;
    }

    public async Task<TopSellersList[]> TopSellersAsync(QueryTopSellerInput dto)
    {
        var db = await this._orderRepository.GetDbContextAsync();

        var query = db.Set<Order>().AsNoTracking();

        query = query.Where(x => x.AffiliateId != null && x.AffiliateId.Value > 0);

        if (dto.StartTime != null)
            query = query.Where(x => x.CreationTime >= dto.StartTime.Value);

        if (dto.EndTime != null)
            query = query.Where(x => x.CreationTime < dto.EndTime.Value);

        var status = (int)OrderStatus.Complete;
        query = query.Where(x => x.OrderStatusId == status);

        var groupedQuery = query.GroupBy(x => x.AffiliateId).Select(x => new
        {
            x.Key,
            totalAmount = x.Sum(d => d.OrderTotal),
            totalQuantity = x.Count()
        });

        groupedQuery = groupedQuery.OrderByDescending(x => x.totalAmount).ThenByDescending(x => x.totalQuantity);

        var count = dto.MaxCount ?? 20;

        var data = await groupedQuery.Take(count).ToArrayAsync();

        var sellers = data.Select(x => new TopSellersList()
        {
            SellerId = x.Key.Value,
            SellerName = String.Empty,
            TotalPrice = x.totalAmount,
            TotalQuantity = x.totalQuantity
        }).ToArray();

        if (sellers.Any())
        {
            var ids = sellers.Select(x => x.SellerId).ToArray();
            var managers = await db.Set<User>().AsNoTracking().Where(x => ids.Contains(x.Id)).ToArrayAsync();

            foreach (var m in sellers)
            {
                var manager = managers.FirstOrDefault(x => x.Id == m.SellerId);
                if (manager == null)
                    continue;
                m.GlobalUserId = manager.GlobalUserId;
            }
        }

        return sellers;
    }

    public async Task<TopCustomersList[]> TopCustomersAsync(QueryTopCustomerInput dto)
    {
        var db = await this._orderRepository.GetDbContextAsync();

        var query = db.Set<Order>().AsNoTracking();

        query = query.Where(x => x.UserId > 0);

        if (dto.StartTime != null)
            query = query.Where(x => x.CreationTime >= dto.StartTime.Value);

        if (dto.EndTime != null)
            query = query.Where(x => x.CreationTime < dto.EndTime.Value);

        var status = (int)OrderStatus.Complete;
        query = query.Where(x => x.OrderStatusId == status);

        var groupedQuery = query.GroupBy(x => x.UserId).Select(x => new
        {
            x.Key,
            totalAmount = x.Sum(d => d.OrderTotal),
            totalQuantity = x.Count()
        });

        groupedQuery = groupedQuery.OrderByDescending(x => x.totalAmount).ThenByDescending(x => x.totalQuantity);

        var count = dto.MaxCount ?? 20;

        var data = await groupedQuery.Take(count).ToArrayAsync();

        var sellers = data.Select(x => new TopCustomersList()
        {
            CustomerId = x.Key,
            CustomerName = String.Empty,
            TotalPrice = x.totalAmount,
            TotalQuantity = x.totalQuantity
        }).ToArray();

        if (sellers.Any())
        {
            var ids = sellers.Select(x => x.CustomerId).ToArray();
            var managers = await db.Set<User>().AsNoTracking().Where(x => ids.Contains(x.Id)).ToArrayAsync();

            foreach (var m in sellers)
            {
                var manager = managers.FirstOrDefault(x => x.Id == m.CustomerId);
                if (manager == null)
                    continue;
                m.GlobalUserId = manager.GlobalUserId;
            }
        }

        return sellers;
    }

    public virtual IList<object> BestSellersReport(int? categoryId = null, int brandId = 0,
        DateTime? startTime = null,
        DateTime? endTime = null, OrderStatus? os = null, PaymentStatus? ps = null, ShippingStatus? ss = null,
        int recordsToReturn = 5, int orderBy = 1, int groupBy = 1, bool showHidden = false)
    {
        throw new NotImplementedException();
        //int? orderStatusId = null;
        //if (os.HasValue)
        //    orderStatusId = (int)os.Value;

        //int? paymentStatusId = null;
        //if (ps.HasValue)
        //    paymentStatusId = (int)ps.Value;

        //int? shippingStatusId = null;
        //if (ss.HasValue)
        //    shippingStatusId = (int)ss.Value;


        //var query1 = from opv in _opvRepository.Table
        //             join o in _orderRepository.Table on opv.OrderId equals o.Id
        //             join p in _goodsRepository.Table on opv.GoodsId equals p.Id
        //             where (!startTime.HasValue || startTime.Value <= o.CreationTime) &&
        //             (!endTime.HasValue || endTime.Value >= o.CreationTime) &&
        //             (!(categoryId.HasValue && categoryId.Value > 0) || categoryId.Value == p.CategoryId) &&
        //             (!(brandId > 0) || brandId == p.BrandId) &&
        //             (!orderStatusId.HasValue || orderStatusId == o.OrderStatusId) &&
        //             (!paymentStatusId.HasValue || paymentStatusId == o.PaymentStatusId) &&
        //             (!shippingStatusId.HasValue || shippingStatusId == o.ShippingStatusId) &&
        //             (!o.IsDeleted) &&
        //             (!p.IsDeleted) &&
        //             (showHidden || p.Published)
        //             select opv;

        //var query2 = groupBy == 1 ?
        //    from opv in query1
        //    group opv by opv.GoodsId into g
        //    select new
        //    {
        //        EntityId = g.Key,
        //        TotalAmount = g.Sum(x => x.Price),
        //        TotalQuantity = g.Sum(x => x.Quantity),
        //    }
        //    :
        //    from opv in query1
        //    group opv by opv.GoodsId into g
        //    select new
        //    {
        //        EntityId = g.Key,
        //        TotalAmount = g.Sum(x => x.Price),
        //        TotalQuantity = g.Sum(x => x.Quantity),
        //    }
        //    ;

        //switch (orderBy)
        //{
        //    case 1:
        //        {
        //            query2 = query2.OrderByDescending(x => x.TotalQuantity);
        //        }
        //        break;
        //    case 2:
        //        {
        //            query2 = query2.OrderByDescending(x => x.TotalAmount);
        //        }
        //        break;
        //    default:
        //        throw new ArgumentException("Wrong orderBy parameter", "orderBy");
        //}

        //if (recordsToReturn != 0 && recordsToReturn != int.MaxValue)
        //    query2 = query2.Take(recordsToReturn);

        //var result = query2.ToList().Select(x =>
        //{
        //    var reportLine = new BestsellersReportLine()
        //    {
        //        EntityId = x.EntityId,
        //        TotalAmount = x.TotalAmount,
        //        TotalQuantity = x.TotalQuantity
        //    };
        //    return reportLine;
        //}).ToList();

        //return result;
    }

    public virtual PagedResponse<Goods> GoodssNeverSold(DateTime? startTime,
        DateTime? endTime, int pageIndex, int pageSize, bool showHidden = false)
    {
        throw new NotImplementedException();
        //var query1 = (from opv in _opvRepository.Table
        //              join o in _orderRepository.Table on opv.OrderId equals o.Id
        //              where (!startTime.HasValue || startTime.Value <= o.CreationTime) &&
        //                    (!endTime.HasValue || endTime.Value >= o.CreationTime) &&
        //                    (!o.IsDeleted)
        //              select opv.GoodsId).Distinct();

        //var query2 = from p in _goodsRepository.Table
        //             where (!query1.Contains(p.Id)) &&
        //                   (!p.IsDeleted) &&
        //                   (showHidden || p.Published)
        //             select p;

        //var query3 = query2.OrderBy(x => x.Id);

        //var list = new PagedResponse<Goods>(query3, pageIndex, pageSize);
        //return list;
    }

    public virtual decimal ProfitReport(List<int> osIds = null, List<int> psIds = null, List<int> ssIds = null,
        DateTime? startTimeUtc = null, DateTime? endTimeUtc = null)
    {
        throw new NotImplementedException();
        //var orders = _orderRepository.Table;
        //if (osIds != null && osIds.Any())
        //    orders = orders.Where(o => osIds.Contains(o.OrderStatusId));
        //if (psIds != null && psIds.Any())
        //    orders = orders.Where(o => psIds.Contains(o.PaymentStatusId));
        //if (ssIds != null && ssIds.Any())
        //    orders = orders.Where(o => ssIds.Contains(o.ShippingStatusId));

        //var query = from opv in _opvRepository.Table
        //            join o in orders on opv.OrderId equals o.Id
        //            join p in _goodsRepository.Table on opv.GoodsId equals p.Id
        //            where (!startTimeUtc.HasValue || startTimeUtc.Value <= o.CreationTime) &&
        //                  (!endTimeUtc.HasValue || endTimeUtc.Value >= o.CreationTime) &&
        //                  (!o.IsDeleted) &&
        //                  (!p.IsDeleted)
        //            select new { opv, p };

        //var profit = Convert.ToDecimal(query.Sum(o => (decimal?)o.opv.Price));
        //return profit;
    }
}