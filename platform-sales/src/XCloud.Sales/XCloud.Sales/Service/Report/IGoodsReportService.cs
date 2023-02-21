using XCloud.Sales.Application;
using XCloud.Sales.Data;
using XCloud.Sales.Data.Domain.Catalog;
using XCloud.Sales.Data.Domain.Logging;
using XCloud.Sales.Service.Catalog;
using XCloud.Sales.Service.Logging;

namespace XCloud.Sales.Service.Report;

public interface IGoodsReportService : ISalesAppService
{
    Task<QuerySearchKeywordsReportResponse[]> QuerySearchKeywordsReportAsync(
        QuerySearchKeywordsReportInput dto);
        
    Task<QueryGoodsVisitReportResponse[]> TopVisitedGoodsAsync(QueryGoodsVisitReportInput dto);

    Task<GoodsPriceHistory[]> QueryCombinationPriceHistoryAsync(QueryCombinationPriceHistoryInput dto);
}

public class GoodsReportService : SalesAppService, IGoodsReportService
{
    private readonly ISalesRepository<GoodsPriceHistory> salesRepository;

    public GoodsReportService(ISalesRepository<GoodsPriceHistory> salesRepository)
    {
        this.salesRepository = salesRepository;
    }

    public async Task<QuerySearchKeywordsReportResponse[]> QuerySearchKeywordsReportAsync(
        QuerySearchKeywordsReportInput dto)
    {
        var db = await this.salesRepository.GetDbContextAsync();

        var logType = (int)ActivityLogType.SearchGoods;
        var logQuery = db.Set<ActivityLog>().AsNoTracking().Where(x => x.ActivityLogTypeId == logType);

        if (dto.UserId != null)
            logQuery = logQuery.Where(x => x.UserId == dto.UserId.Value);

        if (dto.StartTime != null)
            logQuery = logQuery.Where(x => x.CreationTime >= dto.StartTime.Value);

        if (dto.EndTime != null)
            logQuery = logQuery.Where(x => x.CreationTime <= dto.EndTime.Value);

        logQuery = logQuery.Where(x => x.Value != null && x.Value != string.Empty);

        var groupedQuery = logQuery.GroupBy(x => x.Value).Select(x => new { x.Key, count = x.Count() });

        var data = await groupedQuery.OrderByDescending(x => x.count).Take(dto.MaxCount ?? 200).ToArrayAsync();

        var response = data
            .Select(x => new QuerySearchKeywordsReportResponse()
            {
                Keywords = x.Key,
                Count = x.count
            })
            .OrderByDescending(x => x.Count)
            .ToArray();

        return response;
    }

    public async Task<QueryGoodsVisitReportResponse[]> TopVisitedGoodsAsync(QueryGoodsVisitReportInput dto)
    {
        var db = await this.salesRepository.GetDbContextAsync();

        var logType = (int)ActivityLogType.VisitGoods;
        var logQuery = db.Set<ActivityLog>().AsNoTracking().Where(x => x.ActivityLogTypeId == logType);

        if (dto.UserId != null)
            logQuery = logQuery.Where(x => x.UserId == dto.UserId.Value);

        if (dto.StartTime != null)
            logQuery = logQuery.Where(x => x.CreationTime >= dto.StartTime.Value);

        if (dto.EndTime != null)
            logQuery = logQuery.Where(x => x.CreationTime <= dto.EndTime.Value);

        var goodsQuery = db.Set<Goods>().AsNoTracking();

        if (dto.GoodsId != null)
            goodsQuery = goodsQuery.Where(x => x.Id == dto.GoodsId.Value);

        var query = from log in logQuery.Select(x => new { x.SubjectIntId })
            join goods in goodsQuery.Select(x => new { x.Id })
                on log.SubjectIntId equals goods.Id
            select new { log, goods };

        var count = dto.Count ?? 10;

        var groupedQuery = query.GroupBy(x => x.goods.Id).Select(x => new { x.Key, count = x.Count() });

        var data = await groupedQuery.OrderByDescending(x => x.count).Take(count).ToArrayAsync();

        var response = data.Select(x => new QueryGoodsVisitReportResponse()
            { GoodsId = x.Key, VisitedCount = x.count }).ToArray();

        if (response.Any())
        {
            var ids = response.Select(x => x.GoodsId).Distinct().ToArray();
            var goodsList = await db.Set<Goods>().AsNoTracking().Where(x => ids.Contains(x.Id)).ToArrayAsync();
            foreach (var m in response)
            {
                var g = goodsList.FirstOrDefault(x => x.Id == m.GoodsId);
                if (g == null)
                    continue;
                m.Goods = this.ObjectMapper.Map<Goods, GoodsDto>(g);
            }
        }

        return response;
    }

    public async Task<GoodsPriceHistory[]> QueryCombinationPriceHistoryAsync(QueryCombinationPriceHistoryInput dto)
    {
        if (dto.Id <= 0)
            throw new ArgumentNullException(nameof(QueryCombinationPriceHistoryAsync));

        var db = await this.salesRepository.GetDbContextAsync();

        var query = db.Set<GoodsPriceHistory>().AsNoTracking();

        query = query.Where(x => x.GoodsSpecCombinationId == dto.Id);

        if (dto.StartTime != null)
            query = query.Where(x => x.CreationTime >= dto.StartTime.Value);

        if (dto.EndTime != null)
            query = query.Where(x => x.CreationTime < dto.EndTime.Value);

        dto.MaxTake ??= 10000;

        var data = await query.OrderBy(x => x.CreationTime).Take(dto.MaxTake.Value).ToArrayAsync();

        return data;
    }
}