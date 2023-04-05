using XCloud.Core.Application.Entity;
using XCloud.Core.Dto;
using XCloud.Core.Helper;
using XCloud.Database.EntityFrameworkCore.Extensions;
using XCloud.Sales.Application;
using XCloud.Sales.Clients.Platform;
using XCloud.Sales.Data;
using XCloud.Sales.Data.Domain.Orders;
using XCloud.Sales.Data.Domain.Users;

namespace XCloud.Sales.Service.Users;

public interface IUserService : ISalesAppService
{
    Task<User[]> QuerySimplePagingAsync(PagedRequest dto);
    
    Task UpdateStatusAsync(UpdateUserStatusInput dto);

    Task<User> GetUserByIdAsync(int userId);

    Task<PagedResponse<StoreUserDto>> QueryStoreUserPagingAsync(QueryUserInput dto);

    Task<User> GetOrCreateUserByGlobalUserId(string globalUserId);

    Task<User> GetUserByGlobalUserIdAsync(string globalUserId, CachePolicy cachePolicyOption);

    Task TrySetLastActivityTimeAsync(int userId);
}

public class UserService : SalesAppService, IUserService
{
    private readonly ISalesRepository<User> _userRepository;
    private readonly PlatformInternalService _platformInternalService;

    public UserService(ISalesRepository<User> userRepository, PlatformInternalService platformInternalService)
    {
        this._userRepository = userRepository;
        this._platformInternalService = platformInternalService;
    }

    public async Task<User[]> QuerySimplePagingAsync(PagedRequest dto)
    {
        var db = await this._userRepository.GetDbContextAsync();
        var query = db.Set<User>().AsNoTracking();

        var data = await query.OrderBy(x => x.Id).PageBy(dto.ToAbpPagedRequest()).ToArrayAsync();
        return data;
    }

    async Task SetLastActivityTimeAsync(int userId)
    {
        var user = await this._userRepository.QueryOneAsync(x => x.Id == userId);
        if (user == null)
            throw new EntityNotFoundException(nameof(userId));

        user.LastActivityTime = this.Clock.Now;

        await this._userRepository.UpdateAsync(user);
    }

    public async Task TrySetLastActivityTimeAsync(int userId)
    {
        if (userId <= 0)
            return;

        var key = $"mall-try-set-last-activity-time-user-id:{userId}";

        var option = new CacheOption<StoreUserDto>(key, TimeSpan.FromMinutes(1));
        await this.CacheProvider.GetOrSetAsync(async () =>
        {
            //execute resource function
            await this.SetLastActivityTimeAsync(userId);
            return new StoreUserDto() { CreationTime = this.Clock.Now };
        }, option);
    }

    public async Task UpdateStatusAsync(UpdateUserStatusInput dto)
    {
        var db = await this._userRepository.GetDbContextAsync();

        var entity = await db.Set<User>().IgnoreQueryFilters().FirstOrDefaultAsync(x => x.Id == dto.Id);
        if (entity == null)
            throw new EntityNotFoundException(nameof(UpdateStatusAsync));

        if (dto.IsDeleted != null)
            entity.IsDeleted = dto.IsDeleted.Value;

        if (dto.IsActive != null)
            entity.Active = dto.IsActive.Value;

        entity.LastModificationTime = this.Clock.Now;

        await db.TrySaveChangesAsync();
    }

    public virtual async Task<PagedResponse<StoreUserDto>> QueryStoreUserPagingAsync(QueryUserInput dto)
    {
        var db = await this._userRepository.GetDbContextAsync();

        var mappingQuery = from mapping in db.Set<UserGradeMapping>().AsNoTracking()
            join grade in db.Set<UserGrade>().AsNoTracking()
                on mapping.GradeId equals grade.Id
            select new { mapping, grade };

        var now = this.Clock.Now;
        mappingQuery = mappingQuery.Where(x => x.mapping.StartTime == null || x.mapping.StartTime <= now);
        mappingQuery = mappingQuery.Where(x => x.mapping.EndTime == null || x.mapping.EndTime >= now);

        var query = db.Set<User>().IgnoreQueryFilters().AsNoTracking();

        if (dto.IsDeleted != null)
            query = query.Where(c => c.IsDeleted == dto.IsDeleted.Value);

        if (dto.StartTime.HasValue)
            query = query.Where(c => dto.StartTime.Value <= c.CreationTime);

        if (dto.EndTime.HasValue)
            query = query.Where(c => dto.EndTime.Value >= c.CreationTime);

        if (dto.Active.HasValue)
            query = query.Where(c => c.Active == dto.Active.Value);

        if (!string.IsNullOrWhiteSpace(dto.GradeId))
        {
            var userIds = mappingQuery.Where(x => x.grade.Id == dto.GradeId).Select(x => x.mapping.UserId);
            query = query.Where(x => userIds.Contains(x.Id));
        }

        if (ValidateHelper.IsNotEmptyCollection(dto.GlobalUserIds))
            query = query.Where(x => dto.GlobalUserIds.Contains(x.GlobalUserId));

        if (dto.OnlyWithShoppingCart)
            query = query.Where(c => db.Set<ShoppingCartItem>().AsNoTracking().Any(x => x.UserId == c.Id));

        if (!string.IsNullOrWhiteSpace(dto.AccountMobile))
        {
            query = query.Where(x => x.AccountMobile == dto.AccountMobile);
        }

        if (!string.IsNullOrWhiteSpace(dto.Keywords))
        {
            query = query.Where(x => x.NickName.Contains(dto.Keywords));
        }

        var count = 0;
        if (!dto.SkipCalculateTotalCount)
            count = await query.CountAsync();
        var items = await query
            .OrderBy(x => x.IsDeleted)
            .ThenByDescending(x => x.Active)
            .ThenByDescending(x => x.LastActivityTime)
            .ThenByDescending(x => x.CreationTime)
            .PageBy(dto.ToAbpPagedRequest()).ToArrayAsync();

        var userDtos = items.Select(x => this.ObjectMapper.Map<User, StoreUserDto>(x)).ToArray();

        if (userDtos.Any())
        {
            var ids = userDtos.Ids().ToArray();
            var datas = await mappingQuery.Where(x => ids.Contains(x.mapping.UserId)).ToArrayAsync();
            foreach (var m in userDtos)
            {
                var g = datas.FirstOrDefault(x => x.mapping.UserId == m.Id)?.grade;
                if (g == null)
                    continue;
                m.GradeId = g.Id;
                m.GradeName = g.Name;
                m.GradeDescription = g.Description;
            }
        }

        var globalUserIds = userDtos.Where(x => !string.IsNullOrWhiteSpace(x.GlobalUserId))
            .Select(x => x.GlobalUserId).Distinct().ToArray();
        if (globalUserIds.Any())
        {
            var globalUsers = await this._platformInternalService.QueryUserProfileByIdsAsync(globalUserIds);
            foreach (var m in userDtos)
            {
                m.SysUser = globalUsers.FirstOrDefault(x => x.Id == m.GlobalUserId);
            }
        }

        var response = new PagedResponse<StoreUserDto>(userDtos, dto, count);

        return response;
    }

    public virtual async Task<User> GetUserByIdAsync(int userId)
    {
        if (userId <= 0)
            return null;

        var user = await _userRepository.QueryOneAsync(x => x.Id == userId);
        return user;
    }

    public async Task<User> GetUserByGlobalUserIdAsync(string globalUserId)
    {
        var db = await this._userRepository.GetDbContextAsync();

        var set = db.Set<User>();

        var query = set.IgnoreQueryFilters();

        var user = await query.OrderBy(x => x.CreationTime)
            .FirstOrDefaultAsync(x => x.GlobalUserId == globalUserId);

        return user;
    }

    public async Task<User> GetUserByGlobalUserIdAsync(string globalUserId, CachePolicy cachePolicyOption)
    {
        var key = $"sales.user.by.global-user:{globalUserId}";

        var option = new CacheOption<User>(key, TimeSpan.FromMinutes(1)) { CacheCondition = x => x != null };
        var mallUser = await this.CacheProvider.ExecuteWithPolicyAsync(
            () => this.GetUserByGlobalUserIdAsync(globalUserId),
            option,
            cachePolicyOption);

        return mallUser;
    }

    public async Task<User> GetOrCreateUserByGlobalUserId(string globalUserId)
    {
        var db = await this._userRepository.GetDbContextAsync();

        var set = db.Set<User>();

        var user = await set.IgnoreQueryFilters()
            .OrderBy(x => x.CreationTime)
            .FirstOrDefaultAsync(x => x.GlobalUserId == globalUserId);

        if (user == null)
        {
            user = new User
            {
                GlobalUserId = globalUserId,
                CreationTime = this.Clock.Now,
                Active = true,
                IsDeleted = false,
            };
            user.LastActivityTime = user.CreationTime;

            set.Add(user);

            await db.SaveChangesAsync();
        }

        return user;
    }
}