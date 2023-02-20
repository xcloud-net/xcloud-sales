using XCloud.Sales.Data;
using XCloud.Sales.Data.Domain.Logging;
using XCloud.Sales.Data.Domain.Orders;
using XCloud.Sales.Data.Domain.Shipping;
using XCloud.Sales.Data.Domain.Users;
using XCloud.Sales.Services.Users;

namespace XCloud.Sales.Services.Report;

public interface IUserReportService : ISalesAppService
{
    Task<UserActivityGroupByHourResponse[]> UserActivityGroupByHourAsync(UserActivityGroupByHourInput dto);
    
    Task<int> TodayActiveUserCountAsync();
    
    int GetRegisteredUsersReport(int days);
}

public class UserReportService : SalesAppService, IUserReportService
{
    private readonly ISalesRepository<User> _userRepository;
    private readonly ISalesRepository<Order> _orderRepository;
    private readonly IUserService _userService;

    public UserReportService(ISalesRepository<User> userRepository,
        ISalesRepository<Order> orderRepository,
        IUserService userService)
    {
        this._userRepository = userRepository;
        this._orderRepository = orderRepository;
        this._userService = userService;
    }

    public async Task<UserActivityGroupByHourResponse[]> UserActivityGroupByHourAsync(UserActivityGroupByHourInput dto)
    {
        var db = await this._userRepository.GetDbContextAsync();
        var logQuery = db.Set<ActivityLog>().AsNoTracking();

        if (dto.UserId != null)
            logQuery = logQuery.Where(x => x.UserId == dto.UserId.Value);

        if (dto.StartTime != null)
            logQuery = logQuery.Where(x => x.CreationTime >= dto.StartTime.Value);

        if (dto.EndTime != null)
            logQuery = logQuery.Where(x => x.CreationTime <= dto.EndTime.Value);

        var groupedQuery = logQuery.GroupBy(x => new { x.CreationTime.Hour, x.ActivityLogTypeId })
            .Select(x => new
            {
                x.Key.Hour,
                x.Key.ActivityLogTypeId,
                count = x.Count()
            });

        var data = await groupedQuery.OrderBy(x => x.Hour).ToArrayAsync();

        var response = data.Select(x => new UserActivityGroupByHourResponse()
        {
            Hour = x.Hour,
            ActivityType = x.ActivityLogTypeId,
            Count = x.count
        }).ToArray();

        return response;
    }

    public async Task<int> TodayActiveUserCountAsync()
    {
        var db = await this._userRepository.GetDbContextAsync();
        var query = db.Set<User>().AsNoTracking();

        var now = this.Clock.Now;
        var start = now.Date;
        var end = start.AddDays(1);

        query = query.Where(x => x.LastActivityTime >= start && x.LastActivityTime < end);

        var count = await query.Select(x => x.Id).Distinct().CountAsync();
        return count;
    }

    public virtual IList<object> GetBestUsersReport(DateTime? startTime,
        DateTime? endTime, OrderStatus? os, PaymentStatus? ps, ShippingStatus? ss, int orderBy)
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
        //var query1 = from c in _userRepository.Table
        //             join o in _orderRepository.Table on c.Id equals o.UserId
        //             where (!startTime.HasValue || startTime.Value <= o.CreationTime) &&
        //             (!endTime.HasValue || endTime.Value >= o.CreationTime) &&
        //             (!orderStatusId.HasValue || orderStatusId == o.OrderStatusId) &&
        //             (!paymentStatusId.HasValue || paymentStatusId == o.PaymentStatusId) &&
        //             (!shippingStatusId.HasValue || shippingStatusId == o.ShippingStatusId) &&
        //             (!o.IsDeleted) &&
        //             (!c.IsDeleted)
        //             select new { c, o };

        //var query2 = from co in query1
        //             group co by co.c.Id into g
        //             select new
        //             {
        //                 UserId = g.Key,
        //                 OrderTotal = g.Sum(x => x.o.OrderTotal),
        //                 OrderCount = g.Count()
        //             };
        //switch (orderBy)
        //{
        //    case 1:
        //        {
        //            query2 = query2.OrderByDescending(x => x.OrderTotal);
        //        }
        //        break;
        //    case 2:
        //        {
        //            query2 = query2.OrderByDescending(x => x.OrderCount);
        //        }
        //        break;
        //    default:
        //        throw new ArgumentException("Wrong orderBy parameter", "orderBy");
        //}

        //query2 = query2.Take(20);

        //var result = query2.ToList().Select(x =>
        //{
        //    return new BestUserReportLine()
        //    {
        //        UserId = x.UserId,
        //        OrderTotal = x.OrderTotal,
        //        OrderCount = x.OrderCount
        //    };
        //}).ToList();
        //return result;
    }

    public virtual int GetRegisteredUsersReport(int days)
    {
        throw new NotImplementedException();
        //DateTime date = DateTime.Now.AddDays(-days);

        //var query = from c in _userRepository.Table
        //            where !c.IsDeleted &&
        //            c.CreationTime >= date
        //            select c;
        //int count = query.Count();
        //return count;
    }
}