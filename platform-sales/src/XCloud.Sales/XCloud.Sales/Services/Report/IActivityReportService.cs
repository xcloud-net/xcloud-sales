using XCloud.Sales.Data;
using XCloud.Sales.Data.Domain.Logging;

namespace XCloud.Sales.Services.Report;

public interface IActivityReportService : ISalesAppService
{
    Task<UserActivityGroupByGeoLocationResponse[]> GroupByGeoLocationAsync(UserActivityGroupByGeoLocationInput dto);
}

public class ActivityReportService : SalesAppService, IActivityReportService
{
    private readonly ISalesRepository<ActivityLog> _repository;

    public ActivityReportService(ISalesRepository<ActivityLog> repository)
    {
        this._repository = repository;
    }

    public async Task<UserActivityGroupByGeoLocationResponse[]> GroupByGeoLocationAsync(
        UserActivityGroupByGeoLocationInput dto)
    {
        var db = await this._repository.GetDbContextAsync();
        var logQuery = db.Set<ActivityLog>().AsNoTracking();

        if (dto.UserId != null)
            logQuery = logQuery.Where(x => x.UserId == dto.UserId.Value);

        if (dto.StartTime != null)
            logQuery = logQuery.Where(x => x.CreationTime >= dto.StartTime.Value);

        if (dto.EndTime != null)
            logQuery = logQuery.Where(x => x.CreationTime <= dto.EndTime.Value);

        logQuery = logQuery.Where(x => x.GeoCity != null && x.GeoCity != string.Empty);

        var groupedQuery = logQuery.GroupBy(x => new { x.GeoCountry, x.GeoCity }).Select(x => new
        {
            x.Key.GeoCountry,
            x.Key.GeoCity,
            count = x.Count()
        });

        var data = await groupedQuery.OrderByDescending(x => x.count).Take(dto.MaxCount ?? 20).ToArrayAsync();

        var response = data.Select(x => new UserActivityGroupByGeoLocationResponse()
        {
            Country = x.GeoCountry,
            City = x.GeoCity,
            Count = x.count,
        }).ToArray();

        return response;
    }
}