using XCloud.Core.Application.Entity;
using XCloud.Core.Dto;
using XCloud.Database.EntityFrameworkCore.Extensions;
using XCloud.Sales.Application;
using XCloud.Sales.Data;
using XCloud.Sales.Data.Domain.Coupons;
using XCloud.Sales.Data.Domain.Users;
using XCloud.Sales.Service.Users;

namespace XCloud.Sales.Service.Coupons;

public interface ICouponService : ISalesAppService
{
    Task<int> ActiveCouponCountAsync();

    Task CreateUserCouponAsync(CouponUserMappingDto dto);

    Task<CouponDto[]> CheckCanIssueAsync(CouponDto[] data, int userId);

    Task<CouponUserMappingDto[]> AttachUserCouponDataAsync(CouponUserMappingDto[] data, AttachUserCouponDataInput dto);

    Task<CouponDto[]> AttachDataAsync(CouponDto[] data, AttachCouponDataInput dto);

    Task<PagedResponse<CouponUserMappingDto>> QueryUserCouponPagingAsync(QueryUserCouponPagingInput dto);

    Task UpdateCouponInfoAsync(int couponId);

    Task<CouponUserMappingDto> GetUserCouponByIdAsync(int userCouponId);

    Task UseUserCouponAsync(int userCouponId);

    Task ReturnBackUserCouponAsync(int userCouponId);

    Task IssueCouponAsync(int couponId, int userId);

    Task<CouponDto> QueryByIdAsync(int couponId);

    Task UpdateStatusAsync(UpdateCouponStatusInput dto);

    Task UpdateCouponAsync(CouponDto dto);

    Task InsertCouponAsync(CouponDto dto);

    Task<PagedResponse<CouponDto>> QueryPagingAsync(QueryCouponPagingInput dto);
}

public class CouponService : SalesAppService, ICouponService
{
    private readonly ISalesRepository<Coupon> _couponRepository;
    private readonly ISalesRepository<CouponUserMapping> _couponUserRepository;
    private readonly CouponUtils _couponUtils;

    public CouponService(ISalesRepository<Coupon> couponRepository,
        ISalesRepository<CouponUserMapping> couponUserRepository,
        CouponUtils couponUtils)
    {
        this._couponUtils = couponUtils;
        this._couponRepository = couponRepository;
        this._couponUserRepository = couponUserRepository;
    }

    public async Task<int> ActiveCouponCountAsync()
    {
        var db = await this._couponRepository.GetDbContextAsync();
        var query = db.Set<Coupon>().AsNoTracking();

        var now = this.Clock.Now;
        query = query.Where(x => !x.IsDeleted);

        query = query.Where(x => x.StartTime == null || x.StartTime.Value < now);
        query = query.Where(x => x.EndTime == null || x.EndTime.Value > now);

        var count = await query.CountAsync();
        return count;
    }

    public async Task<CouponUserMappingDto[]> AttachUserCouponDataAsync(CouponUserMappingDto[] data,
        AttachUserCouponDataInput dto)
    {
        if (!data.Any())
            return data;

        var db = await this._couponRepository.GetDbContextAsync();

        if (dto.User)
        {
            var userIds = data.Select(x => x.UserId).Distinct().ToArray();
            var users = await db.Set<User>().AsNoTracking().WhereIdIn(userIds).ToArrayAsync();
            foreach (var m in data)
            {
                var u = users.FirstOrDefault(x => x.Id == m.UserId);
                if (u == null)
                    continue;

                m.User = this.ObjectMapper.Map<User, StoreUserDto>(u);
            }
        }

        return data;
    }


    public async Task<CouponDto[]> CheckCanIssueAsync(CouponDto[] data, int userId)
    {
        if (!data.Any())
            return data;

        var db = await this._couponRepository.GetDbContextAsync();
        var ids = data.Ids().Distinct().ToArray();

        var query = db.Set<CouponUserMapping>().AsNoTracking()
            .Where(x => x.UserId == userId)
            .Where(x => ids.Contains(x.CouponId));

        var groupedQuery = query
            .GroupBy(x => new { x.CouponId, x.IsUsed })
            .Select(x => new
            {
                x.Key.CouponId,
                x.Key.IsUsed,
                Count = x.Count()
            });

        var groupedData = await groupedQuery.ToArrayAsync();

        foreach (var m in data)
        {
            //user can issue by default
            m.CanBeIssued = true;

            var couponGroupedData = groupedData.Where(x => x.CouponId == m.Id).ToArray();

            //coupon in user pocket
            if (couponGroupedData.Any(x => x.IsUsed && x.Count > 0))
            {
                m.CanBeIssued = false;
                continue;
            }

            //if store administrator set account limitation
            if (m.AccountLimitationIsSet(out var limitation))
            {
                var count = couponGroupedData.Sum(x => x.Count);
                //check if reach the limitation
                m.CanBeIssued = count < limitation;
            }
        }

        return data;
    }

    public async Task<CouponDto[]> AttachDataAsync(CouponDto[] data, AttachCouponDataInput dto)
    {
        if (!data.Any())
            return data;

        var db = await this._couponRepository.GetDbContextAsync();

        var ids = data.Ids().Distinct().ToArray();

        if (dto.IssuedToUserId != null)
        {
            await this.CheckCanIssueAsync(data, userId: dto.IssuedToUserId.Value);
        }

        return data;
    }

    public async Task<CouponUserMappingDto> GetUserCouponByIdAsync(int userCouponId)
    {
        if (userCouponId <= 0)
            throw new ArgumentNullException(nameof(userCouponId));

        var db = await this._couponRepository.GetDbContextAsync();

        var entity =
            await db.Set<CouponUserMapping>().AsNoTracking().FirstOrDefaultAsync(x => x.Id == userCouponId);

        if (entity == null)
            throw new EntityNotFoundException(nameof(GetUserCouponByIdAsync));

        return this.ObjectMapper.Map<CouponUserMapping, CouponUserMappingDto>(entity);
    }

    public async Task UseUserCouponAsync(int userCouponId)
    {
        var db = await this._couponRepository.GetDbContextAsync();

        var userCoupon = await db.Set<CouponUserMapping>().FirstOrDefaultAsync(x => x.Id == userCouponId);
        if (userCoupon == null)
            throw new EntityNotFoundException(nameof(UseUserCouponAsync));

        if (userCoupon.IsUsed)
            throw new UserFriendlyException("优惠券已经被使用");

        var now = this.Clock.Now;

        if (this._couponUtils.IsUserCouponExpired(userCoupon))
        {
            throw new UserFriendlyException("优惠券已经过期");
        }

        userCoupon.IsUsed = true;
        userCoupon.UsedTime = now;

        await db.SaveChangesAsync();

        await this.UpdateCouponInfoAsync(userCoupon.CouponId);
    }

    public async Task ReturnBackUserCouponAsync(int userCouponId)
    {
        var db = await this._couponRepository.GetDbContextAsync();

        var userCoupon = await db.Set<CouponUserMapping>().FirstOrDefaultAsync(x => x.Id == userCouponId);
        if (userCoupon == null)
            throw new EntityNotFoundException(nameof(UseUserCouponAsync));

        if (!userCoupon.IsUsed)
            throw new UserFriendlyException("优惠券未被使用");

        userCoupon.IsUsed = false;
        userCoupon.UsedTime = null;

        await db.SaveChangesAsync();

        await this.UpdateCouponInfoAsync(userCoupon.CouponId);
    }

    public async Task UpdateCouponInfoAsync(int couponId)
    {
        var db = await this._couponRepository.GetDbContextAsync();

        var entity = await this._couponRepository.QueryOneAsync(x => x.Id == couponId);
        if (entity == null)
            throw new EntityNotFoundException(nameof(UpdateCouponInfoAsync));

        var query = db.Set<CouponUserMapping>().AsNoTracking()
            .Where(x => x.CouponId == entity.Id);

        entity.IssuedAmount = await query.CountAsync();
        entity.UsedAmount = await query.Where(x => x.IsUsed).CountAsync();

        await this._couponRepository.UpdateAsync(entity);
    }

    public async Task CreateUserCouponAsync(CouponUserMappingDto dto)
    {
        if (dto == null)
            throw new ArgumentNullException(nameof(dto));
        if (dto.UserId <= 0)
            throw new ArgumentNullException(nameof(dto.UserId));
        if (decimal.Equals(dto.Value, default))
            throw new ArgumentNullException(nameof(dto.Value));

        var entity = this.ObjectMapper.Map<CouponUserMappingDto, CouponUserMapping>(dto);

        entity.Id = default;
        entity.IsUsed = false;
        entity.UsedTime = null;
        entity.CreationTime = this.Clock.Now;

        await this._couponUserRepository.InsertAsync(entity);
    }

    public async Task IssueCouponAsync(int couponId, int userId)
    {
        if (couponId <= 0 || userId <= 0)
            throw new ArgumentNullException(nameof(IssueCouponAsync));

        var db = await this._couponRepository.GetDbContextAsync();

        var entity = await this._couponRepository.QueryOneAsync(x => x.Id == couponId);
        if (entity == null)
            throw new EntityNotFoundException(nameof(IssueCouponAsync));

        var now = this.Clock.Now;
        if ((entity.StartTime == null || entity.StartTime.Value <= now) &&
            (entity.EndTime == null || entity.EndTime.Value >= now))
        {
            //not expired
        }
        else
        {
            throw new UserFriendlyException("优惠券已经过期");
        }

        if (entity.IsAmountLimit)
        {
            if (entity.Amount <= 0)
            {
                throw new UserFriendlyException("优惠券已经发放完毕");
            }

            entity.Amount -= 1;
            entity.Amount = Math.Max(0, entity.Amount);
        }
        else
        {
            //unlimited coupon, do nothing
        }

        if (entity.AccountLimitationIsSet(out var limitation))
        {
            var userCouponCount =
                await this._couponUserRepository.CountAsync(x => x.UserId == userId && x.CouponId == entity.Id);
            if (userCouponCount >= limitation)
                throw new UserFriendlyException("you reach the limitation for get this coupon");
        }

        var userCoupon = new CouponUserMapping()
        {
            Id = default,
            CouponId = entity.Id,
            Value = entity.Value,
            MinimumConsumption = entity.MinimumConsumption,
            UserId = userId,
            IsUsed = false,
            UsedTime = null,
            ExpiredAt = null,
            CreationTime = now
        };
        if (entity.ExpiredDaysFromIssue != null)
            userCoupon.ExpiredAt = now.AddDays(entity.ExpiredDaysFromIssue.Value);

        db.Set<CouponUserMapping>().Add(userCoupon);

        await db.SaveChangesAsync();
    }

    public async Task<CouponDto> QueryByIdAsync(int couponId)
    {
        if (couponId <= 0)
            return null;

        var entity = await this._couponRepository.QueryOneAsync(x => x.Id == couponId);
        if (entity == null)
            return null;

        return this.ObjectMapper.Map<Coupon, CouponDto>(entity);
    }

    public async Task UpdateStatusAsync(UpdateCouponStatusInput dto)
    {
        var db = await this._couponRepository.GetDbContextAsync();

        var entity = await db.Set<Coupon>().IgnoreQueryFilters().FirstOrDefaultAsync(x => x.Id == dto.Id);

        if (entity == null)
            throw new EntityNotFoundException(nameof(entity));

        if (dto.IsDeleted != null)
            entity.IsDeleted = dto.IsDeleted.Value;

        await db.TrySaveChangesAsync();
    }

    private async Task CheckInsertInputAsync(CouponDto dto)
    {
        if (dto == null)
            throw new ArgumentNullException(nameof(dto));

        if (string.IsNullOrWhiteSpace(dto.Title))
            throw new UserFriendlyException(nameof(dto.Title));

        if (dto.Value <= 0)
            throw new UserFriendlyException(nameof(dto.Value));

        await Task.CompletedTask;
    }

    public async Task UpdateCouponAsync(CouponDto dto)
    {
        if (dto == null || dto.Id <= 0)
            throw new ArgumentNullException(nameof(dto));

        var db = await this._couponRepository.GetDbContextAsync();

        var entity = await db.Set<Coupon>().FirstOrDefaultAsync(x => x.Id == dto.Id);

        if (entity == null)
            throw new EntityNotFoundException(nameof(UpdateCouponAsync));

        entity.Title = dto.Title;
        entity.Value = dto.Value;
        entity.StartTime = dto.StartTime;
        entity.EndTime = dto.EndTime;
        entity.MinimumConsumption = dto.MinimumConsumption;
        entity.ExpiredDaysFromIssue = dto.ExpiredDaysFromIssue;
        entity.Amount = dto.Amount;
        entity.IsAmountLimit = dto.IsAmountLimit;
        entity.AccountIssuedLimitCount = dto.AccountIssuedLimitCount;

        await db.TrySaveChangesAsync();
    }

    public async Task InsertCouponAsync(CouponDto dto)
    {
        await this.CheckInsertInputAsync(dto);

        var db = await this._couponRepository.GetDbContextAsync();

        var entity = this.ObjectMapper.Map<CouponDto, Coupon>(dto);

        entity.Id = default;
        entity.CreationTime = this.Clock.Now;

        await this._couponRepository.InsertAsync(entity);
        await this.CurrentUnitOfWork.SaveChangesAsync();
    }

    public async Task<PagedResponse<CouponDto>> QueryPagingAsync(QueryCouponPagingInput dto)
    {
        var db = await this._couponRepository.GetDbContextAsync();

        var query = db.Set<Coupon>().IgnoreQueryFilters().AsNoTracking();

        if (dto.IsDeleted != null)
            query = query.Where(x => x.IsDeleted == dto.IsDeleted.Value);

        if (dto.AvailableFor != null)
        {
            query = query.Where(x => x.StartTime == null || x.StartTime.Value < dto.AvailableFor.Value);
            query = query.Where(x => x.EndTime == null || x.EndTime.Value > dto.AvailableFor.Value);
        }

        var count = 0;
        if (!dto.SkipCalculateTotalCount)
            count = await query.CountAsync();

        if (dto.SortForAdmin ?? false)
        {
            query = query.OrderBy(x => x.IsDeleted).ThenByDescending(x => x.CreationTime);
        }
        else
        {
            query = query.OrderBy(x => x.IsDeleted).ThenByDescending(x => x.CreationTime);
        }

        var data = await query.PageBy(dto.ToAbpPagedRequest()).ToArrayAsync();

        var response = data.Select(x => this.ObjectMapper.Map<Coupon, CouponDto>(x)).ToArray();

        return new PagedResponse<CouponDto>(response, dto, count);
    }

    public async Task<PagedResponse<CouponUserMappingDto>> QueryUserCouponPagingAsync(QueryUserCouponPagingInput dto)
    {
        var db = await this._couponRepository.GetDbContextAsync();

        var query = db.Set<CouponUserMapping>().IgnoreQueryFilters().AsNoTracking();

        if (dto.UserId != null)
            query = query.Where(x => x.UserId == dto.UserId);

        if (dto.Used != null)
            query = query.Where(x => x.IsUsed == dto.Used.Value);

        if (dto.AvailableFor != null)
            query = query.Where(x => x.ExpiredAt == null || x.ExpiredAt.Value > dto.AvailableFor.Value);

        var count = 0;
        if (!dto.SkipCalculateTotalCount)
            count = await query.CountAsync();

        query = query.OrderBy(x => x.IsUsed).ThenByDescending(x => x.CreationTime);

        var data = await query.PageBy(dto.ToAbpPagedRequest()).ToArrayAsync();

        var response = data.Select(x => this.ObjectMapper.Map<CouponUserMapping, CouponUserMappingDto>(x)).ToArray();

        return new PagedResponse<CouponUserMappingDto>(response, dto, count);
    }
}