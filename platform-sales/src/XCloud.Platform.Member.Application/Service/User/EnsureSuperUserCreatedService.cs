using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Volo.Abp.Application.Dtos;
using Volo.Abp.DependencyInjection;
using XCloud.Core.Dto;
using XCloud.Platform.Core.Application;
using XCloud.Platform.Core.Domain.Admin;
using XCloud.Platform.Core.Domain.User;
using XCloud.Platform.Data.Database;
using XCloud.Platform.Member.Application.Job;
using XCloud.Platform.Member.Application.Service.Admin;
using XCloud.Platform.Shared;
using XCloud.Platform.Shared.Helper;
using XCloud.Redis;

namespace XCloud.Platform.Member.Application.Service.User;

public class CreateSeedDataInput : IEntityDto
{
    public string UserName { get; set; }
    public string UserMobilePhone { get; set; }
    public string IdCardNo { get; set; }
    public string AvatarUrl { get; set; } = "https://joeschmoe.io/api/v1/random";
}

[UnitOfWork]
[ExposeServices(typeof(EnsureSuperUserCreatedService))]
public class EnsureSuperUserCreatedService : PlatformApplicationService
{
    private readonly IMemberRepository<SysUser> _repository;
    private readonly MemberHelper _memberHelper;
    private readonly IAdminAccountService _adminAccountService;
    private readonly IUserMobileService _userMobileService;
    private readonly IUserIdCardService _userIdCardService;
    private readonly IUserAccountService _userAccountService;
    private readonly IUserProfileService _userProfileService;

    public EnsureSuperUserCreatedService(
        IAdminAccountService adminAccountService,
        IUserMobileService userMobileService,
        IUserIdCardService userIdCardService,
        IUserAccountService userAccountService,
        IUserProfileService userProfileService,
        IMemberRepository<SysUser> repository,
        MemberHelper memberHelper)
    {
        _adminAccountService = adminAccountService;
        _userMobileService = userMobileService;
        _userIdCardService = userIdCardService;
        _userAccountService = userAccountService;
        _userProfileService = userProfileService;
        _repository = repository;
        _memberHelper = memberHelper;
    }

    public virtual async Task EnsureSuperUserCreatedAsync()
    {
        var dto = new CreateSeedDataInput()
        {
            UserName = IdentityConsts.Account.DefaultUserName,
            UserMobilePhone = "13915280232",
            IdCardNo = "320623199406255621"
        };

        using var redLock = await RedLockClient.RedLockFactory.CreateLockAsync(
            resource:
            $"ensure.super.user.created.username={dto.UserName}",
            expiryTime: TimeSpan.FromSeconds(30));

        if (redLock.IsAcquired)
        {
            //添加测试数据
            var user = await CreateUserAsync(dto);
            //admin identity
            var admin = await CreateAdminAccountAsync(user, dto);
        }
        else
        {
            throw new FailToGetRedLockException("获取分布式锁失败");
        }

        this.Logger.LogInformation("创建默认账号");
    }

    private async Task<SysUser> CreateUserAsync(CreateSeedDataInput dto)
    {
        var normalizeIdentityName = this._memberHelper.NormalizeIdentityName(dto.UserName);

        var db = await this._repository.GetDbContextAsync();

        var user = await db.Set<SysUser>()
            .IgnoreQueryFilters().AsNoTracking()
            .FirstOrDefaultAsync(x => x.IdentityName == normalizeIdentityName);

        if (user == null)
        {
            var res = await _userAccountService.CreateUserAccountAsync(new IdentityNameDto(dto.UserName));
            res.ThrowIfErrorOccured();

            user = res.Data;

            if (!string.IsNullOrWhiteSpace(dto.UserMobilePhone))
            {
                await this._userMobileService.SetUserMobilePhoneAsync(user.Id, dto.UserMobilePhone);
            }

            if (!string.IsNullOrWhiteSpace(dto.IdCardNo))
            {
                await this._userIdCardService.SetUserIdCardAsync(new SetIdCardInput()
                {
                    UserId = user.Id,
                    IdCard = dto.IdCardNo,
                    RealName = "wj"
                });
            }

            await _userAccountService.SetPasswordAsync(user.Id, "123");
            await _userProfileService.UpdateNickNameAsync(user.Id, "super-user");
            await _userProfileService.UpdateAvatarAsync(user.Id, dto.AvatarUrl);
        }

        await this._userAccountService.UpdateUserStatusAsync(new UpdateUserStatusDto()
        {
            Id = user.Id,
            IsActive = true,
            IsDeleted = false
        });

        return user;
    }

    private async Task<SysAdmin> CreateAdminAccountAsync(SysUser user, CreateSeedDataInput dto)
    {
        var db = await this._repository.GetDbContextAsync();

        var admin = await db.Set<SysAdmin>().IgnoreQueryFilters().AsNoTracking()
            .OrderBy(x => x.CreationTime)
            .FirstOrDefaultAsync(x => x.UserId == user.Id);

        if (admin == null)
        {
            var res = await _adminAccountService.CreateAdminAccountAsync(new CreateAdminDto(user.Id));
            res.ThrowIfErrorOccured();

            admin = res.Data;
        }

        await _adminAccountService.UpdateAdminStatusAsync(new UpdateAdminStatusDto()
        {
            Id = admin.Id,
            IsActive = true,
            IsSuperAdmin = true
        });

        return admin;
    }
}