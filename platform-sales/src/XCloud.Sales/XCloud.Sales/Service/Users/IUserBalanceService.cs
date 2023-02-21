using XCloud.Core.Dto;
using XCloud.Redis;
using XCloud.Sales.Application;
using XCloud.Sales.Data;
using XCloud.Sales.Data.Domain.Users;

namespace XCloud.Sales.Service.Users;

public enum BalanceActionType : int
{
    None = 0,
    Add = 1,
    Use = -1
}

public interface IUserBalanceService : ISalesAppService
{
    Task<decimal> CountAllBalanceAsync();

    Task InsertBalanceHistoryAsync(BalanceHistoryDto dto);

    Task<PagedResponse<BalanceHistoryDto>> QueryPagingAsync(QueryBalanceInput dto);
}

public class UserBalanceService : SalesAppService, IUserBalanceService
{
    private readonly ISalesRepository<BalanceHistory> _salesRepository;

    public UserBalanceService(ISalesRepository<BalanceHistory> salesRepository)
    {
        this._salesRepository = salesRepository;
    }

    public async Task<decimal> CountAllBalanceAsync()
    {
        var db = await this._salesRepository.GetDbContextAsync();

        var sum = await db.Set<User>().AsNoTracking().SumAsync(x => x.Balance);

        return sum;
    }

    public async Task<PagedResponse<BalanceHistoryDto>> QueryPagingAsync(QueryBalanceInput dto)
    {
        var db = await this._salesRepository.GetDbContextAsync();

        var query = db.Set<BalanceHistory>().AsNoTracking();

        query = query.Where(x => x.UserId == dto.UserId);

        if (dto.ActionType != null)
            query = query.Where(x => x.ActionType == dto.ActionType.Value);

        var count = 0;
        if (!dto.SkipCalculateTotalCount)
            count = await query.CountAsync();

        var list = await query.OrderByDescending(x => x.CreationTime).PageBy(dto.AsAbpPagedRequestDto()).ToArrayAsync();
        var items = list.Select(x => this.ObjectMapper.Map<BalanceHistory, BalanceHistoryDto>(x)).ToArray();

        return new PagedResponse<BalanceHistoryDto>(items, dto, count);
    }

    public async Task InsertBalanceHistoryAsync(BalanceHistoryDto dto)
    {
        if (dto == null)
            throw new ArgumentNullException(nameof(InsertBalanceHistoryAsync));
        if (dto.UserId <= 0)
            throw new ArgumentNullException(nameof(dto.UserId));

        dto.Balance = Math.Abs(dto.Balance);
        if (dto.Balance == decimal.Zero)
            return;

        using var dlock = await this.RedLockClient.RedLockFactory.CreateLockAsync(
            resource: $"{nameof(UserPointService)}.{nameof(InsertBalanceHistoryAsync)}.{dto.UserId}",
            expiryTime: TimeSpan.FromSeconds(3));

        if (dlock.IsAcquired)
        {
            var db = await this._salesRepository.GetDbContextAsync();

            var offset = default(decimal);
            if (dto.ActionType == (int)BalanceActionType.Use)
                offset = -dto.Balance;
            else if (dto.ActionType == (int)BalanceActionType.Add)
                offset = dto.Balance;
            else
                throw new ArgumentException("points action type");

            var user = await db.Set<User>().FirstOrDefaultAsync(x => x.Id == dto.UserId);
            if (user == null)
                throw new EntityNotFoundException(nameof(InsertBalanceHistoryAsync));

            user.Balance += offset;
            if (user.Balance < 0)
                throw new UserFriendlyException("balance is not enough to deduct");

            dto.LatestBalance = user.Balance;

            var entity = this.ObjectMapper.Map<BalanceHistoryDto, BalanceHistory>(dto);
            entity.Id = this.GuidGenerator.CreateGuidString();
            entity.CreationTime = this.Clock.Now;

            db.Set<BalanceHistory>().Add(entity);

            await db.SaveChangesAsync();
        }
        else
        {
            throw new FailToGetRedLockException("can't get dlock");
        }
    }
}