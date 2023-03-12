using System.Collections.Generic;
using System.Threading.Tasks;
using FluentAssertions;
using Masuit.Tools;
using Microsoft.EntityFrameworkCore;
using Volo.Abp.Domain.Entities;
using XCloud.Application.Service;
using XCloud.Core.Application;
using XCloud.Core.Dto;
using XCloud.Core.IdGenerator;
using XCloud.Database.EntityFrameworkCore.Extensions;
using XCloud.Platform.Core.Application;
using XCloud.Platform.Core.Domain.User;
using XCloud.Platform.Data.Database;
using XCloud.Platform.Member.Application.Queue;

namespace XCloud.Platform.Member.Application.Service.User;

/// <summary>
/// 用户手机号
/// </summary>
public interface IUserMobileService : IXCloudApplicationService
{
    string NormalizeMobilePhone(string mobile);

    Task<bool> IsPhoneExistAsync(string phone);

    Task<ApiResponse<SysUserIdentity>> SetUserMobilePhoneAsync(string userId, string phone);

    Task<IEnumerable<UserMobilePhoneDto>> GetUserMobilePhonesAsync(string userId);

    Task SendSmsAsync(UserPhoneBindSmsMessage message);

    /// <summary>
    /// 读取用户
    /// </summary>
    Task<SysUser> GetUserByPhoneAsync(string phone);

    Task RemoveUserMobilesAsync(string userId);

    Task ConfirmMobileAsync(string identityId);
}

public class UserMobileService : PlatformApplicationService, IUserMobileService
{
    private readonly IMemberRepository<SysUserIdentity> _userMobileRepository;
    private readonly IMemberShipMessageBus _memberShipMessageBus;

    public UserMobileService(IMemberRepository<SysUserIdentity> userMobileRepository, IMemberShipMessageBus memberShipMessageBus)
    {
        this._userMobileRepository = userMobileRepository;
        _memberShipMessageBus = memberShipMessageBus;
    }
    
    public int IdentityType => (int)UserIdentityTypeEnum.MobilePhone;

    public string NormalizeMobilePhone(string mobile)
    {
        return this.MemberHelper.NormalizeMobilePhone(mobile);
    }

    public async Task RemoveUserMobilesAsync(string userId)
    {
        var db = await this._userMobileRepository.GetDbContextAsync();

        var identityType = this.IdentityType;

        var set = db.Set<SysUserIdentity>();

        var mobiles = await set.Where(x => x.IdentityType == identityType && x.UserId == userId).ToArrayAsync();

        if (mobiles.Any())
            set.RemoveRange(mobiles);

        await db.TrySaveChangesAsync();
    }

    void CheckMobilePhone(string mobilePhone)
    {
        if (string.IsNullOrWhiteSpace(mobilePhone))
            throw new UserFriendlyException("手机号码不能为空");

        if (!mobilePhone.MatchPhoneNumber())
            throw new UserFriendlyException("手机号格式不对");
    }

    public async Task<IEnumerable<UserMobilePhoneDto>> GetUserMobilePhonesAsync(string userId)
    {
        userId.Should().NotBeNullOrEmpty("get user phone useruid");

        var db = await this._userMobileRepository.GetDbContextAsync();

        var identityType = this.IdentityType;

        var data = await db.Set<SysUserIdentity>().AsNoTracking()
            .Where(x => x.UserId == userId && x.IdentityType == identityType)
            .OrderByDescending(x => x.CreationTime)
            .ToArrayAsync();

        var res = data.Select(x => new UserMobilePhoneDto()
        {
            UserId = x.UserId,
            MobilePhone = x.MobilePhone,
            MobilePhoneConfirmed = x.MobileConfirmed
        }).ToArray();

        return res;
    }

    public async Task<SysUser> GetUserByPhoneAsync(string phone)
    {
        phone = MemberHelper.NormalizeMobilePhone(phone);

        var db = await this._userMobileRepository.GetDbContextAsync();

        var query = from userIdentity in db.Set<SysUserIdentity>().AsNoTracking()
            join u in db.Set<SysUser>().AsNoTracking()
                on userIdentity.UserId equals u.Id into userGrouping
            from user in userGrouping.DefaultIfEmpty()
            select new { userIdentity, user };

        var identityType = this.IdentityType;

        query = query.Where(
            x => x.userIdentity.IdentityType == identityType && x.userIdentity.UserIdentity == phone);

        var data = await query.FirstOrDefaultAsync();

        if (data == null)
            return null;

        return data.user;
    }

    public async Task<bool> IsPhoneExistAsync(string phone)
    {
        phone.Should().NotBeNullOrEmpty("is phone exist phone");
        phone = MemberHelper.NormalizeMobilePhone(phone);

        var db = await this._userMobileRepository.GetDbContextAsync();

        var identityType = this.IdentityType;

        var res = await db.Set<SysUserIdentity>().AsNoTracking().IgnoreQueryFilters()
            .Where(x => x.IdentityType == identityType && x.UserIdentity == phone)
            .AnyAsync();

        return res;
    }

    public async Task ConfirmMobileAsync(string identityId)
    {
        if (string.IsNullOrWhiteSpace(identityId))
            throw new ArgumentNullException(nameof(ConfirmMobileAsync));

        var db = await this._userMobileRepository.GetDbContextAsync();

        var set = db.Set<SysUserIdentity>();

        var identityType = this.IdentityType;

        var mobile = await set.FirstOrDefaultAsync(x => x.Id == identityId && x.IdentityType == identityType);
        if (mobile == null)
            throw new EntityNotFoundException(nameof(ConfirmMobileAsync));

        mobile.MobileConfirmed = true;
        mobile.MobileConfirmedTimeUtc = this.Clock.Now;

        await db.TrySaveChangesAsync();
    }

    public async Task<ApiResponse<SysUserIdentity>> SetUserMobilePhoneAsync(string uid, string phone)
    {
        uid.Should().NotBeNullOrEmpty("set phone uid");
        phone.Should().NotBeNullOrEmpty("set phone phone");

        phone = MemberHelper.NormalizeMobilePhone(phone);

        CheckMobilePhone(phone);

        if (await this.IsPhoneExistAsync(phone))
            return new ApiResponse<SysUserIdentity>().SetError("mobile phone is already exist");

        var db = await this._userMobileRepository.GetDbContextAsync();

        var set = db.Set<SysUserIdentity>();

        var identityType = this.IdentityType;

        var data = await set.Where(x => x.UserId == uid && x.IdentityType == identityType).ToArrayAsync();
        if (data.Any())
            set.RemoveRange(data);

        var map = new SysUserIdentity()
        {
            UserId = uid,
            UserIdentity = phone,
            MobilePhone = phone,
            MobileAreaCode = "",
            MobileConfirmed = false,
            MobileConfirmedTimeUtc = null,
            IdentityType = identityType
        };

        map.Id = this.GuidGenerator.CreateGuidString();
        map.CreationTime = this.Clock.Now;

        set.Add(map);

        await db.SaveChangesAsync();

        return new ApiResponse<SysUserIdentity>(map);
    }

    public async Task SendSmsAsync(UserPhoneBindSmsMessage message)
    {
        await this._memberShipMessageBus.SendSms(message);
    }
}