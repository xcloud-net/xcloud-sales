using XCloud.Core.Dto;
using XCloud.Database.EntityFrameworkCore.Extensions;
using XCloud.Redis;
using XCloud.Sales.Data;
using XCloud.Sales.Data.Domain.Users;

namespace XCloud.Sales.Services.Users;

public interface IPrepaidCardService : ISalesAppService
{
    Task UsePrepaidCardAsync(UsePrepaidCardInput dto);

    Task<decimal> QueryUnusedAmountTotalAsync();

    Task<PrepaidCardDto> QueryByIdAsync(string cardId);

    Task<PrepaidCard> CreatePrepaidCardAsync(PrepaidCardDto dto);

    Task UpdateStatusAsync(UpdatePrepaidCardStatusInput dto);

    Task<PagedResponse<PrepaidCardDto>> QueryPagingAsync(QueryPrepaidCardPagingInput dto);
}

public class PrepaidCardService : SalesAppService, IPrepaidCardService
{
    private readonly ISalesRepository<PrepaidCard> _salesRepository;
    private readonly IUserBalanceService _userBalanceService;

    public PrepaidCardService(ISalesRepository<PrepaidCard> salesRepository,
        IUserBalanceService userBalanceService)
    {
        this._salesRepository = salesRepository;
        this._userBalanceService = userBalanceService;
    }

    public async Task UsePrepaidCardAsync(UsePrepaidCardInput dto)
    {
        if (dto == null || string.IsNullOrWhiteSpace(dto.CardId) || dto.UserId <= 0)
            throw new ArgumentNullException(nameof(UsePrepaidCardAsync));

        using var dlock = await this.RedLockClient.RedLockFactory.CreateLockAsync(
            resource: $"{nameof(PrepaidCardService)}.{nameof(UsePrepaidCardAsync)}.{dto.CardId}",
            expiryTime: TimeSpan.FromSeconds(5));

        if (dlock.IsAcquired)
        {
            using var uow = this.UnitOfWorkManager.Begin(requiresNew: true, isTransactional: true);

            try
            {
                var db = await this._salesRepository.GetDbContextAsync();

                var now = this.Clock.Now;

                var card = await db.Set<PrepaidCard>().FirstOrDefaultAsync(x => x.Id == dto.CardId);
                if (card == null)
                    throw new UserFriendlyException("card is not exist");

                if (card.Used)
                    throw new UserFriendlyException("card is used");

                if (card.EndTime != null && card.EndTime.Value < now)
                    throw new UserFriendlyException("card is expired");

                var user = await db.Set<User>().FirstOrDefaultAsync(x => x.Id == dto.UserId);
                if (user == null)
                    throw new UserFriendlyException("use is not exist");

                card.UserId = user.Id;
                card.UsedTime = now;
                card.Used = true;

                await this._userBalanceService.ChangeUserBalanceAsync(
                    user.Id, card.Amount, BalanceActionType.Add,
                    "charge from prepaid card");

                await db.TrySaveChangesAsync();

                await uow.CompleteAsync();
            }
            catch
            {
                await uow.RollbackAsync();
                throw;
            }
        }
        else
        {
            throw new FailToGetRedLockException(nameof(UsePrepaidCardAsync));
        }
    }

    public async Task<decimal> QueryUnusedAmountTotalAsync()
    {
        var db = await this._salesRepository.GetDbContextAsync();
        var query = db.Set<PrepaidCard>().AsNoTracking();

        var total = await query.Where(x => !x.Used).SumAsync(x => x.Amount);
        return total;
    }

    public async Task<PagedResponse<PrepaidCardDto>> QueryPagingAsync(QueryPrepaidCardPagingInput dto)
    {
        if (dto == null)
            throw new ArgumentNullException(nameof(QueryPagingAsync));

        var db = await this._salesRepository.GetDbContextAsync();
        var query = db.Set<PrepaidCard>().AsNoTracking();

        if (dto.Used != null)
            query = query.Where(x => x.Used == dto.Used.Value);

        if (dto.UserId != null)
            query = query.Where(x => x.UserId == dto.UserId.Value);

        var count = 0;
        if (!dto.SkipCalculateTotalCount)
            count = await query.CountAsync();

        var items = await query
            .OrderBy(x => x.Used).ThenByDescending(x => x.CreationTime)
            .PageBy(dto.AsAbpPagedRequestDto())
            .ToArrayAsync();

        var dtos = items.Select(x => this.ObjectMapper.Map<PrepaidCard, PrepaidCardDto>(x)).ToArray();

        return new PagedResponse<PrepaidCardDto>(dtos, dto, count);
    }

    public async Task UpdateStatusAsync(UpdatePrepaidCardStatusInput dto)
    {
        if (dto == null || string.IsNullOrWhiteSpace(dto.Id))
            throw new ArgumentNullException(nameof(UpdateStatusAsync));

        var db = await this._salesRepository.GetDbContextAsync();

        var entity = await db.Set<PrepaidCard>().IgnoreQueryFilters().FirstOrDefaultAsync(x => x.Id == dto.Id);

        if (entity == null)
            throw new EntityNotFoundException(nameof(UpdateStatusAsync));

        if (dto.IsActive != null)
            entity.IsActive = dto.IsActive.Value;

        if (dto.IsDeleted != null)
            entity.IsDeleted = dto.IsDeleted.Value;

        await db.TrySaveChangesAsync();
    }

    public async Task<PrepaidCard> CreatePrepaidCardAsync(PrepaidCardDto dto)
    {
        if (dto == null)
            throw new ArgumentNullException(nameof(CreatePrepaidCardAsync));
        if (dto.Amount <= 0)
            throw new ArgumentException(nameof(dto.Amount));

        var db = await this._salesRepository.GetDbContextAsync();

        var entity = this.ObjectMapper.Map<PrepaidCardDto, PrepaidCard>(dto);
        entity.Used = false;
        entity.UserId = default;
        entity.UsedTime = null;
        entity.IsDeleted = false;
        entity.IsActive = true;

        entity.Id = this.GuidGenerator.CreateGuidString();
        entity.CreationTime = this.Clock.Now;

        db.Set<PrepaidCard>().Add(entity);

        await db.SaveChangesAsync();

        return entity;
    }

    public async Task<PrepaidCardDto> QueryByIdAsync(string cardId)
    {
        if (string.IsNullOrWhiteSpace(cardId))
            throw new ArgumentNullException(nameof(QueryByIdAsync));

        var db = await this._salesRepository.GetDbContextAsync();

        var entity = await db.Set<PrepaidCard>().AsNoTracking().FirstOrDefaultAsync(x => x.Id == cardId);

        if (entity == null)
            return null;

        return this.ObjectMapper.Map<PrepaidCard, PrepaidCardDto>(entity);
    }

}