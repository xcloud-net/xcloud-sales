using XCloud.Sales.Application;
using XCloud.Sales.Data;
using XCloud.Sales.Data.Domain.Aftersale;
using XCloud.Sales.Data.Domain.Orders;
using XCloud.Sales.Data.Domain.Users;
using XCloud.Sales.Service.Users;

namespace XCloud.Sales.Service.Report;

public interface IAfterSalesReportService : ISalesAppService
{
    Task<TopAfterSaleMallUsersResponse[]> TopAfterSalesMallUsersAsync(TopAfterSaleMallUsersInput dto);
}

public class AfterSalesReportService : SalesAppService, IAfterSalesReportService
{
    private readonly ISalesRepository<AfterSales> _repository;

    public AfterSalesReportService(ISalesRepository<AfterSales> repository)
    {
        this._repository = repository;
    }

    public async Task<TopAfterSaleMallUsersResponse[]> TopAfterSalesMallUsersAsync(TopAfterSaleMallUsersInput dto)
    {
        var db = await this._repository.GetDbContextAsync();

        var query = from aftersaleItem in db.Set<AftersalesItem>().AsNoTracking()
            join aftersale in db.Set<AfterSales>().AsNoTracking()
                on aftersaleItem.AftersalesId equals aftersale.Id
            join order_item in db.Set<OrderItem>().AsNoTracking()
                on aftersaleItem.OrderItemId equals order_item.Id
            select new { aftersale_item = aftersaleItem, aftersale, order_item };

        if (dto.StartTime != null)
            query = query.Where(x => x.aftersale.CreationTime >= dto.StartTime.Value);
        
        if (dto.EndTime != null)
            query = query.Where(x => x.aftersale.CreationTime <= dto.EndTime.Value);

        var groupedQuery = query.GroupBy(x => new
        {
            x.aftersale.UserId,
        }).Select(x => new
        {
            x.Key.UserId,
            count = x.Select(d => d.aftersale.Id).Distinct().Count(),
            amount = x.Sum(d => d.order_item.Price)
        });

        var data = await groupedQuery.OrderByDescending(x => x.count).ThenByDescending(x => x.amount)
            .Take(dto.MaxCount ?? 100)
            .ToArrayAsync();

        var response = data.Select(x => new TopAfterSaleMallUsersResponse()
        {
            UserId = x.UserId,
            Count = x.count,
            Amount = x.amount
        }).ToArray();

        if (response.Any())
        {
            var userids = response.Select(x => x.UserId).Distinct().ToArray();
            var users = await db.Set<User>().IgnoreQueryFilters().AsNoTracking().Where(x => userids.Contains(x.Id))
                .ToArrayAsync();
            foreach (var m in response)
            {
                var u = users.FirstOrDefault(x => x.Id == m.UserId);
                if (u == null)
                    continue;
                m.User = this.ObjectMapper.Map<User, StoreUserDto>(u);
            }
        }

        return response;
    }
}