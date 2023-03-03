using MaxMind.GeoIP2;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using UAParser;
using XCloud.Core.Dto;
using XCloud.Redis;
using XCloud.Sales.Application;
using XCloud.Sales.Clients.Platform;
using XCloud.Sales.Core;
using XCloud.Sales.Data;
using XCloud.Sales.Data.Domain.Logging;
using XCloud.Sales.Data.Domain.Users;
using XCloud.Sales.Service.Users;

namespace XCloud.Sales.Service.Logging;

public enum ActivityLogType : int
{
    VisitGoods = 1,
    SearchGoods = 2,
    PlaceOrder = 3,
    VisitPage = 5,
    GetCoupon = 6,
    AddShoppingCart = 7,
    DeleteShoppingCart = 8,
    AddFavorite = 9,
    DeleteFavorite = 10,
    AuditLog = 11,
}

public static class ActivityLogSubjectType
{
    public const string Goods = "goods";
    public const string GoodsCombination = "goods-combination";
    public const string Order = "order";
    public const string Coupon = "coupon";
    public const string Page = "page";
}

public interface IActivityLogService : ISalesAppService
{
    Task<ActivityLog> TryResolveGeoAddressAsync(ActivityLog log);
        
    Task<PagedResponse<ActivityLogDto>> QueryPagingAsync(ActivityLogSearchInput dto);

    ActivityLog AttachHttpContextInfo(ActivityLog log);

    Task InsertAsync(ActivityLog activityLog);

    Task DeleteAsync(int id);

    Task ClearExpiredDataWithLockAsync(DateTime endTime);
}

public class ActivityLogService : SalesAppService, IActivityLogService
{
    private readonly ISalesRepository<ActivityLog> _activityLogRepository;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IWebHelper _webHelper;
    private readonly PlatformInternalService _platformInternalService;
    private readonly IConfiguration _configuration;

    public ActivityLogService(
        IConfiguration configuration,
        PlatformInternalService platformInternalService,
        ISalesRepository<ActivityLog> activityLogRepository,
        IHttpContextAccessor httpContextAccessor,
        IWebHelper webHelper)
    {
        this._configuration = configuration;
        this._platformInternalService = platformInternalService;
        this._activityLogRepository = activityLogRepository;
        this._httpContextAccessor = httpContextAccessor;
        this._webHelper = webHelper;
    }

    public async Task<ActivityLog> TryResolveGeoAddressAsync(ActivityLog log)
    {
        var geoDatabasePath = this._configuration["app:config:geo_database"];
        if (!string.IsNullOrWhiteSpace(log.IpAddress) && !string.IsNullOrWhiteSpace(geoDatabasePath))
        {
            try
            {
                using var reader = new DatabaseReader(geoDatabasePath);
                var response = reader.City(log.IpAddress);

                log.GeoCountry = response.Country.Name;
                log.GeoCity = response.City.Name;
                log.Lng = response.Location.Longitude;
                log.Lat = response.Location.Latitude;
            }
            catch (Exception e)
            {
                this.Logger.LogError(message: e.Message, exception: e);
            }
        }

        await Task.CompletedTask;
        return log;
    }

    public ActivityLog AttachHttpContextInfo(ActivityLog log)
    {
        var context = this._httpContextAccessor.HttpContext;
        if (context != null)
        {
            log.RequestPath = context.Request.Path;
            log.UrlReferrer = context.Request.Headers.Referer;
            log.UserAgent = context.Request.Headers.UserAgent;
            log.IpAddress = this._webHelper.GetCurrentIpAddress();

            try
            {
                var parser = Parser.GetDefault();
                var ua = parser.Parse(log.UserAgent);
                if (ua != null)
                {
                    log.Device = ua.OS.Family;
                    log.BrowserType = ua.UA.Family;
                }
            }
            catch (Exception e)
            {
                this.Logger.LogError(exception: e, message: e.Message);
            }
        }

        return log;
    }

    private async Task<bool> IgnoreActivityByKeyAsync(string key, TimeSpan timeToExpired)
    {
        var ignore = true;
        var option = new CacheOption<IdDto>(key, timeToExpired);

        await this.CacheProvider.GetOrSetAsync(async () =>
        {
            ignore = false;
            return await Task.FromResult(new IdDto());
        }, option);

        return ignore;
    }

    private async Task<bool> IgnoreActivityAsync(ActivityLog log)
    {
        if (log.ActivityLogTypeId == (int)ActivityLogType.SearchGoods)
        {
            var key = $"activity.log.{log.ActivityLogTypeId}.{log.UserId}.{log.Value}";
            if (await this.IgnoreActivityByKeyAsync(key, TimeSpan.FromMinutes(1)))
                return true;
        }
        else if (log.ActivityLogTypeId == (int)ActivityLogType.VisitGoods)
        {
            var key = $"activity.log.{log.ActivityLogTypeId}.{log.UserId}.{log.SubjectIntId}";
            if (await this.IgnoreActivityByKeyAsync(key, TimeSpan.FromMinutes(1)))
                return true;
        }
        else if (log.ActivityLogTypeId == (int)ActivityLogType.VisitPage)
        {
            var key = $"activity.log.{log.ActivityLogTypeId}.{log.UserId}.{log.SubjectId}";
            if (await this.IgnoreActivityByKeyAsync(key, TimeSpan.FromMinutes(1)))
                return true;
        }

        return false;
    }

    public virtual async Task InsertAsync(ActivityLog activityLog)
    {
        if (activityLog == null)
            throw new ArgumentNullException(nameof(activityLog));

        if (await this.IgnoreActivityAsync(activityLog))
            return;

        activityLog.Id = default;
        activityLog.CreationTime = this.Clock.Now;
        await this._activityLogRepository.InsertAsync(activityLog);
    }

    public virtual async Task DeleteAsync(int id)
    {
        if (id <= 0)
            throw new ArgumentNullException(nameof(id));

        var log = await this._activityLogRepository.QueryOneAsync(x => x.Id == id);

        if (log == null)
            return;

        await this._activityLogRepository.DeleteAsync(log);
    }

    public virtual async Task<PagedResponse<ActivityLogDto>> QueryPagingAsync(ActivityLogSearchInput dto)
    {
        var db = await this._activityLogRepository.GetDbContextAsync();

        var query = from log in db.Set<ActivityLog>().AsNoTracking()
            join u in db.Set<User>().AsNoTracking()
                on log.UserId equals u.Id into userGrouping
            from user in userGrouping.DefaultIfEmpty()
            select new { log, user };

        if (dto.ActivityLogTypeId != null)
            query = query.Where(x => x.log.ActivityLogTypeId == dto.ActivityLogTypeId.Value);

        if (dto.UserId != null)
            query = query.Where(x => x.log.UserId == dto.UserId.Value);

        if (dto.StartTime != null)
            query = query.Where(x => x.log.CreationTime >= dto.StartTime.Value);

        if (dto.EndTime != null)
            query = query.Where(x => x.log.CreationTime <= dto.EndTime.Value);

        var count = 0;
        if (!dto.SkipCalculateTotalCount)
            count = await query.CountAsync();

        var data = await query.OrderByDescending(x => x.log.CreationTime).PageBy(dto.ToAbpPagedRequest())
            .ToArrayAsync();

        var adminIds = data.Select(x => x.log.AdministratorId).Where(x => !string.IsNullOrWhiteSpace(x)).Distinct()
            .ToArray();

        var adminList = await this._platformInternalService.QueryAdminProfileByIdsAsync(adminIds);

        ActivityLogDto BuildResponse(ActivityLog log, User userOrNull)
        {
            var logDto = this.ObjectMapper.Map<ActivityLog, ActivityLogDto>(log);
            logDto.Admin = adminList.FirstOrDefault(x => x.Id == logDto.AdministratorId);
            if (userOrNull != null)
                logDto.User = this.ObjectMapper.Map<User, StoreUserDto>(userOrNull);
            return logDto;
        }

        var items = data.Select(x => BuildResponse(x.log, x.user)).ToArray();

        //var admins = items.Select(x => x.Admin).WhereNotNull().ToArray();
        //await this._platformInternalService.AttachSysUserAsync(admins);

        var users = items.Select(x => x.User).WhereNotNull().ToArray();

        await this._platformInternalService.AttachSysUserAsync(users);

        return new PagedResponse<ActivityLogDto>(items, dto, count);
    }

    public virtual async Task ClearExpiredDataWithLockAsync(DateTime endTime)
    {
        using var @lock = await this.RedLockClient.RedLockFactory.CreateLockAsync(
            resource: $"mall-clear-expired-activity-log",
            expiryTime: TimeSpan.FromMinutes(1),
            waitTime: TimeSpan.FromSeconds(10),
            retryTime: TimeSpan.FromSeconds(1));

        if (@lock.IsAcquired)
        {
            var db = await this._activityLogRepository.GetDbContextAsync();

            var maxId = 0;
            var batchSize = 200;

            while (true)
            {
                var set = db.Set<ActivityLog>();

                var list = await set
                    .Where(x => x.CreationTime <= endTime)
                    .Where(x => x.Id > maxId)
                    .OrderBy(x => x.Id)
                    .Take(batchSize).ToArrayAsync();

                if (!list.Any())
                    break;

                set.RemoveRange(list);

                await db.SaveChangesAsync();

                maxId = list.Max(x => x.Id);
            }
        }
        else
        {
            throw new FailToGetRedLockException(nameof(ClearExpiredDataWithLockAsync));
        }
    }
}