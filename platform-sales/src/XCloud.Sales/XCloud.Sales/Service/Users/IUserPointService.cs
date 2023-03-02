using XCloud.Core.Dto;
using XCloud.Redis;
using XCloud.Sales.Application;
using XCloud.Sales.Data;
using XCloud.Sales.Data.Domain.Users;

namespace XCloud.Sales.Service.Users;

public enum PointsActionType : int
{
    None = 0,
    Add = 1,
    Use = -1
}

public interface IUserPointService : ISalesAppService
{
    Task AddPointsHistoryAsync(PointsHistoryDto dto);

    Task<PagedResponse<PointsHistoryDto>> QueryPagingAsync(QueryPointsInput dto);
}

public class UserPointService : SalesAppService, IUserPointService
{
    private readonly ISalesRepository<PointsHistory> _pointsRepository;

    public UserPointService(ISalesRepository<PointsHistory> pointsRepository)
    {
        this._pointsRepository = pointsRepository;
    }

    public async Task<PagedResponse<PointsHistoryDto>> QueryPagingAsync(QueryPointsInput dto)
    {
        var db = await this._pointsRepository.GetDbContextAsync();

        var query = db.Set<PointsHistory>().AsNoTracking();

        query = query.Where(x => x.UserId == dto.UserId);

        if (!string.IsNullOrWhiteSpace(dto.OrderId))
            query = query.Where(x => x.OrderId == dto.OrderId);

        if (dto.ActionType != null)
            query = query.Where(x => x.ActionType == dto.ActionType.Value);

        var count = 0;
        if (!dto.SkipCalculateTotalCount)
            count = await query.CountAsync();

        var list = await query.OrderByDescending(x => x.CreationTime).PageBy(dto.ToAbpPagedRequest()).ToArrayAsync();
        var items = list.Select(x => this.ObjectMapper.Map<PointsHistory, PointsHistoryDto>(x)).ToArray();

        return new PagedResponse<PointsHistoryDto>(items, dto, count);
    }

    public async Task AddPointsHistoryAsync(PointsHistoryDto dto)
    {
        if (dto == null)
            throw new ArgumentNullException(nameof(dto));
        if (dto.UserId <= 0)
            throw new ArgumentNullException(nameof(dto.UserId));

        dto.Points = Math.Abs(dto.Points);
        if (dto.Points == 0)
            throw new ArgumentNullException(nameof(dto.Points));

        using var dlock = await this.RedLockClient.RedLockFactory.CreateLockAsync(
            resource: $"{nameof(UserPointService)}.{nameof(AddPointsHistoryAsync)}.{dto.UserId}",
            expiryTime: TimeSpan.FromSeconds(3));

        if (dlock.IsAcquired)
        {
            var db = await this._pointsRepository.GetDbContextAsync();

            if (!string.IsNullOrWhiteSpace(dto.OrderId))
                if (await db.Set<PointsHistory>().AnyAsync(x => x.OrderId == dto.OrderId))
                {
                    this.Logger.LogInformation("order points is sent");
                    return;
                }

            var offset = 0;
            if (dto.ActionType == (int)PointsActionType.Use)
                offset = -dto.Points;
            else if (dto.ActionType == (int)PointsActionType.Add)
                offset = dto.Points;
            else
                throw new ArgumentException("points action type");

            var entity = this.ObjectMapper.Map<PointsHistoryDto, PointsHistory>(dto);
                
            var user = await db.Set<User>().FirstOrDefaultAsync(x => x.Id == entity.UserId);
            if (user == null)
                throw new EntityNotFoundException(nameof(AddPointsHistoryAsync));

            user.Points += offset;
            if (offset > 0)
                user.HistoryPoints += offset;
            user.LastModificationTime = this.Clock.Now;

            entity.PointsBalance = user.Points;
            entity.CreationTime = this.Clock.Now;

            db.Set<PointsHistory>().Add(entity);

            await db.SaveChangesAsync();
        }
        else
        {
            throw new FailToGetRedLockException("can't get dlock");
        }
    }
}