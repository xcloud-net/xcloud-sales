using System.Collections.Generic;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Volo.Abp.Domain.Entities;
using Volo.Abp.Validation;
using XCloud.Core.Application;
using XCloud.Core.Dto;
using XCloud.Core.Helper;
using XCloud.Database.EntityFrameworkCore.Crud;
using XCloud.Platform.Core.Application;
using XCloud.Platform.Core.Domain.Admin;
using XCloud.Platform.Core.Domain.User;
using XCloud.Platform.Data.Database;
using XCloud.Platform.Member.Application.Service.Admin;

namespace XCloud.Platform.Member.Application.Service.User;

/// <summary>
/// 用户个人资料
/// </summary>
public interface IUserProfileService : IXCloudApplicationService
{
    Task<SysUserDto[]> QueryTopUsersAsync(QueryUserDto dto);

    Task<SysUserDto[]> AttachDataAsync(SysUserDto[] data, AttachUserDataInput dto);

    Task<SysUserDto[]> QueryUserProfileByIdsAsync(string[] ids);

    Task<PagedResponse<SysUserDto>> QueryUserProfilePaginationAsync(QueryUserDto dto);

    Task UpdateProfileAsync(SysUserDto model);

    Task UpdateNickNameAsync(string userId, string nickName);

    Task UpdateAvatarAsync(string userId, string avatarUrl);

    Task<SysUserDto> QueryProfileByUserIdAsync(IdDto queryUserByIdDto);
    Task<SysUserDto> QueryProfileByUserIdAsync(IdDto dto, CachePolicy cachePolicyOption);
}

public class UserProfileService : PlatformApplicationService, IUserProfileService
{
    private readonly IMemberRepository<SysUser> _userProfileRepository;

    public UserProfileService(IMemberRepository<SysUser> userRepo)
    {
        _userProfileRepository = userRepo;
    }

    public virtual async Task UpdateProfileAsync(SysUserDto dto)
    {
        if (dto == null || string.IsNullOrWhiteSpace(dto.Id))
            throw new ArgumentNullException(nameof(UpdateProfileAsync));

        var user = await _userProfileRepository.QueryOneAsync(x => x.Id == dto.Id);
        if (user == null)
            throw new EntityNotFoundException($"用户不存在:{dto.Id}");

        user.SetEntityFields(new
        {
            dto.Avatar,
            dto.Gender,
            dto.NickName
        });

        user.LastModificationTime = this.Clock.Now;

        if (!user.IsValid(out var msg))
            throw new AbpValidationException(msg);

        await _userProfileRepository.UpdateAsync(user);
    }

    public async Task UpdateNickNameAsync(string userId, string nickName)
    {
        if (string.IsNullOrWhiteSpace(userId))
            throw new ArgumentNullException(nameof(UpdateNickNameAsync));

        var db = await this._userProfileRepository.GetDbContextAsync();

        var res = await db.UpdateEntity<SysUser>()
            .Where(x => x.Id == userId)
            .SetField(x => x.NickName, nickName)
            .SetUpdateTime(this.Clock.Now)
            .ExecuteAsync();

        (res > 0).Should().BeTrue();
    }

    public async Task UpdateAvatarAsync(string user_uid, string avatar_url)
    {
        if (string.IsNullOrWhiteSpace(user_uid))
            throw new ArgumentNullException(nameof(UpdateAvatarAsync));

        var user = await _userProfileRepository.QueryOneAsync(x => x.Id == user_uid);
        if (user == null)
            throw new EntityNotFoundException($"用户不存在:{user_uid}");

        user.Avatar = avatar_url;
        user.LastModificationTime = this.Clock.Now;

        await _userProfileRepository.UpdateNowAsync(user);
    }

    async Task<IEnumerable<SysUserIdentity>> QueryUserIdentity(string keyword, int top)
    {
        var db = await this._userProfileRepository.GetDbContextAsync();

        var set = db.Set<SysUserIdentity>().AsNoTracking();

        var scope = new[]
        {
            (int)UserIdentityTypeEnum.Email,
            (int)UserIdentityTypeEnum.MobilePhone
        };

        var query = set.Where(x => scope.Contains(x.IdentityType)).Where(x => x.UserIdentity.StartsWith(keyword));

        query = query.OrderByDescending(x => x.CreationTime).Take(top);

        var res = await query.ToArrayAsync();

        return res;
    }

    public async Task<SysUserDto[]> AttachDataAsync(SysUserDto[] data, AttachUserDataInput dto)
    {
        if (!data.Any())
            return data;

        var db = await _userProfileRepository.GetDbContextAsync();

        var ids = data.Ids().ToArray();

        if (dto.Mobile)
        {
            var identityType = (int)UserIdentityTypeEnum.MobilePhone;
            var mobileList = await db.Set<SysUserIdentity>().AsNoTracking()
                .Where(x => ids.Contains(x.UserId) && x.IdentityType == identityType)
                .Where(x => x.MobileConfirmed != null && x.MobileConfirmed.Value)
                .ToArrayAsync();

            foreach (var m in data)
            {
                m.AccountMobile = mobileList.Where(x => x.UserId == m.Id).Select(x => x.MobilePhone).FirstOrDefault();
            }
        }

        return data;
    }

    async Task<IQueryable<string>> BuildUserIdsQuery(DbContext db, QueryUserDto dto)
    {
        await Task.CompletedTask;

        var query = db.Set<SysUser>().AsNoTracking().IgnoreQueryFilters();

        if (!string.IsNullOrWhiteSpace(dto.Keyword))
            query = query.Where(x => x.IdentityName == dto.Keyword || x.NickName.Contains(dto.Keyword));

        if (dto.IsDeleted != null)
            query = query.Where(x => x.IsDeleted == dto.IsDeleted.Value);

        var idsQuery = query.Select(x => x.Id);
        return idsQuery;
    }

    public async Task<SysUserDto[]> QueryTopUsersAsync(QueryUserDto dto)
    {
        var db = await _userProfileRepository.GetDbContextAsync();

        var ids = await this.BuildUserIdsQuery(db, dto);

        var query = db.Set<SysUser>().AsNoTracking().IgnoreQueryFilters();

        query = query.Where(x => ids.Contains(x.Id));

        var users = await query.OrderByDescending(x => x.CreationTime).Take(dto.Take ?? 20).ToArrayAsync();

        return users.Select(x => this.ObjectMapper.Map<SysUser, SysUserDto>(x)).ToArray();
    }

    public async Task<PagedResponse<SysUserDto>> QueryUserProfilePaginationAsync(QueryUserDto dto)
    {
        var db = await _userProfileRepository.GetDbContextAsync();

        var query = db.Set<SysUser>().AsNoTracking().IgnoreQueryFilters();

        if (!string.IsNullOrWhiteSpace(dto.Keyword))
        {
            var identity = await this.QueryUserIdentity(dto.Keyword, 20);
            var uids = identity.Select(x => x.UserId).ToArray();

            query = query.Where(x => x.IdentityName == dto.Keyword || x.NickName == dto.Keyword || uids.Contains(x.Id));
        }

        if (dto.IsDeleted != null)
        {
            if (dto.IsDeleted.Value)
            {
                query = query.Where(x => x.IsDeleted == true);
            }
            else
            {
                query = query.Where(x => x.IsDeleted == false);
            }
        }

        if (dto.IsActive != null)
            query = query.Where(x => x.IsActive == dto.IsActive.Value);

        var joinQuery = from user in query
            join a in db.Set<SysAdmin>().IgnoreQueryFilters().AsNoTracking()
                on user.Id equals a.UserId into adminGrouping
            from admin in adminGrouping.DefaultIfEmpty()
            select new { user, admin };

        var count = 0;
        if (!dto.SkipCalculateTotalCount)
            count = await joinQuery.CountAsync();

        var list = await joinQuery
            .OrderBy(x=>x.user.IsDeleted)
            .ThenByDescending(x=>x.user.IsActive)
            .ThenByDescending(x => x.user.CreationTime)
            .PageBy(dto.ToAbpPagedRequest())
            .ToArrayAsync();

        SysUserDto BuildResponse(SysUser user, SysAdmin adminOrNull)
        {
            var userDto = this.ObjectMapper.Map<SysUser, SysUserDto>(user);
            if (adminOrNull != null)
                userDto.AdminIdentity = this.ObjectMapper.Map<SysAdmin, SysAdminDto>(adminOrNull);
            return userDto;
        }

        var items = list.Select(x => BuildResponse(x.user, x.admin)).ToArray();

        return new PagedResponse<SysUserDto>(items, dto, count);
    }

    public async Task<SysUserDto> QueryProfileByUserIdAsync(IdDto dto,CachePolicy cachePolicyOption)
    {
        var key = $"sys-user-profile-user:{dto.Id}";
        var sysUser = await this.CacheProvider.ExecuteWithPolicyAsync(() => this.QueryProfileByUserIdAsync(dto),
            new CacheOption<SysUserDto>(key,TimeSpan.FromMinutes(3)),
            cachePolicyOption);
        return sysUser;
    }

    public async Task<SysUserDto> QueryProfileByUserIdAsync(IdDto dto)
    {
        var db = await this._userProfileRepository.GetDbContextAsync();

        var query = from user in db.Set<SysUser>().AsNoTracking()

            select new { user };

        query = query.Where(x => x.user.Id == dto.Id);

        var data = await query.FirstOrDefaultAsync();
        if (data == null)
        {
            return null;
        }

        var profile = this.ObjectMapper.Map<SysUser, SysUserDto>(data.user);

        await this.AttachDataAsync(new[] { profile }, new AttachUserDataInput() { Mobile = true });

        return profile;
    }

    public async Task<SysUserDto[]> QueryUserProfileByIdsAsync(string[] ids)
    {
        if (ValidateHelper.IsEmptyCollection(ids))
            return Array.Empty<SysUserDto>();

        var data = await _userProfileRepository.QueryManyAsync(x => ids.Contains(x.Id));

        var res = data.Select(x => this.ObjectMapper.Map<SysUser, SysUserDto>(x)).ToArray();

        await this.AttachDataAsync(res, new AttachUserDataInput() { Mobile = true});
            
        return res;
    }
}