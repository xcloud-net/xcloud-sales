using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Volo.Abp.Domain.Entities;
using XCloud.Application.Service;
using XCloud.Core.Dto;
using XCloud.Core.IdGenerator;
using XCloud.Platform.Core.Application;
using XCloud.Platform.Core.Domain.User;
using XCloud.Platform.Data.Database;

namespace XCloud.Platform.Application.Member.Service.User;

public interface IUserAccountService : IXCloudApplicationService
{
    Task<UserAuthResponse> GetUserAuthResultAsync(string userId);

    Task<SysUserDto> QueryUserAccountAsync(string accountIdentity);

    Task UpdateUserStatusAsync(UpdateUserStatusDto dto);

    Task<bool> IsUserIdentityNameExistAsync(IdentityNameDto dto);

    Task<ApiResponse<SysUser>> PasswordLoginAsync(PasswordLoginDto dto);

    Task<ApiResponse<SysUser>> CreateUserAccountAsync(IdentityNameDto dto);

    Task SetPasswordAsync(string userId, string password);

    Task<SysUserDto> GetUserDtoByIdAsync(IdDto dto);

    Task<SysUserDto> GetUserDtoByIdAsync(IdDto filter, CachePolicy cachePolicyOption);

    Task<SysUser> GetUserByIdentityNameAsync(IdentityNameDto dto);
}

public class UserAccountService : PlatformApplicationService, IUserAccountService
{
    private readonly IMemberRepository<SysUser> _userAccountRepository;

    public UserAccountService(IMemberRepository<SysUser> userRepo)
    {
        _userAccountRepository = userRepo;
    }

    public async Task<UserAuthResponse> GetUserAuthResultAsync(string userId)
    {
        if (string.IsNullOrWhiteSpace(userId))
            throw new ArgumentNullException(nameof(userId));

        var res = new UserAuthResponse();

        var userData = await this.GetUserDtoByIdAsync(userId,
            new CachePolicy() { Cache = true });

        if (userData == null)
        {
            res.SetError("user not exist");
            return res;
        }

        if (!userData.IsActive)
        {
            res.IsNotActive = true;
            res.SetError("user is not active");
            return res;
        }

        if (userData.IsDeleted)
        {
            res.IsDeleted = true;
            res.SetError("user is deleted");
            return res;
        }

        res.SetData(userData);
        return res;
    }

    public async Task<SysUserDto> QueryUserAccountAsync(string accountIdentity)
    {
        if (string.IsNullOrWhiteSpace(accountIdentity))
            return null;

        var db = await this._userAccountRepository.GetDbContextAsync();

        var query = from user in db.Set<SysUser>().AsNoTracking()
            join i in db.Set<SysUserIdentity>().AsNoTracking()
                on user.Id equals i.UserId into identityGrouping
            from identity in identityGrouping.DefaultIfEmpty()
            select new { user, identity };

        var normalizedIdentityName = this.MemberHelper.NormalizeIdentityName(accountIdentity);

        var mobileIdentityType = (int)UserIdentityTypeEnum.MobilePhone;
        query = query.Where(x => x.user.IdentityName == normalizedIdentityName ||
                                 (x.identity.IdentityType == mobileIdentityType &&
                                  x.identity.MobilePhone == accountIdentity));

        var data = await query.FirstOrDefaultAsync();

        if (data == null)
            return null;

        return this.ObjectMapper.Map<SysUser, SysUserDto>(data.user);
    }

    public virtual async Task<ApiResponse<SysUser>> PasswordLoginAsync(PasswordLoginDto dto)
    {
        dto.IdentityName.Should().NotBeNullOrEmpty();
        dto.Password.Should().NotBeNullOrEmpty();

        var data = new ApiResponse<SysUser>();

        var normalizedIdentityName = this.MemberHelper.NormalizeIdentityName(dto.IdentityName);

        var db = await this._userAccountRepository.GetDbContextAsync();

        var userEntity = await db.Set<SysUser>().IgnoreQueryFilters().AsNoTracking()
            .FirstOrDefaultAsync(x => x.IdentityName == normalizedIdentityName);

        if (userEntity == null)
        {
            data.SetError("用户不存在");
            return data;
        }

        if (userEntity.PassWord != MemberHelper.EncryptPassword(dto.Password))
        {
            data.SetError("密码错误");
            return data;
        }

        if (userEntity.IsDeleted)
        {
            data.SetError("account is removed");
            return data;
        }

        if (!userEntity.IsActive)
        {
            data.SetError("account is disabled");
            return data;
        }

        data.SetData(userEntity);
        return data;
    }

    public async Task<ApiResponse<SysUser>> CreateUserAccountAsync(IdentityNameDto model)
    {
        if (model == null || string.IsNullOrWhiteSpace(model.IdentityName))
            throw new ArgumentNullException(nameof(CreateUserAccountAsync));

        var data = new ApiResponse<SysUser>();

        var entity = new SysUser();

        MemberHelper.NormalizeIdentityName(entity, model.IdentityName);
        entity.Id = this.GuidGenerator.CreateGuidString();
        entity.CreationTime = this.Clock.Now;
        entity.IsActive = true;
        entity.IsDeleted = false;

        if (!MemberHelper.ValidateIdentityName(model.IdentityName, out var msg))
        {
            return data.SetError(msg);
        }

        //检查用户名是否存在
        if (await IsUserIdentityNameExistAsync(model.IdentityName))
        {
            return data.SetError("用户名已存在");
        }

        await _userAccountRepository.InsertNowAsync(entity);
        data.SetData(entity);
        return data;
    }

    public virtual async Task SetPasswordAsync(string uid, string pwd)
    {
        if (string.IsNullOrWhiteSpace(uid) || string.IsNullOrWhiteSpace(pwd))
            throw new ArgumentNullException(nameof(SetPasswordAsync));

        var user = await _userAccountRepository.QueryOneAsync(x => x.Id == uid);
        if (user == null)
            throw new EntityNotFoundException("用户不存在，无法修改密码");

        user.PassWord = MemberHelper.EncryptPassword(pwd);

        var now = this.Clock.Now;

        user.LastPasswordUpdateTime = now;
        user.LastModificationTime = now;

        await _userAccountRepository.UpdateNowAsync(user);
    }

    public async Task UpdateUserStatusAsync(UpdateUserStatusDto dto)
    {
        if (string.IsNullOrWhiteSpace(dto?.Id))
            throw new ArgumentNullException(nameof(UpdateUserStatusAsync));

        var db = await this._userAccountRepository.GetDbContextAsync();

        var entity = await db.Set<SysUser>().FirstOrDefaultAsync(x => x.Id == dto.Id);
        if (entity == null)
            throw new EntityNotFoundException(nameof(entity));

        if (dto.IsDeleted != null)
        {
            entity.IsDeleted = dto.IsDeleted.Value;
        }

        if (dto.IsActive != null)
        {
            entity.IsActive = dto.IsActive.Value;
        }

        entity.LastModificationTime = this.Clock.Now;

        await _userAccountRepository.UpdateNowAsync(entity);
    }

    public virtual async Task<SysUser> GetUserByIdentityNameAsync(IdentityNameDto dto)
    {
        if (string.IsNullOrWhiteSpace(dto?.IdentityName))
            throw new ArgumentNullException(nameof(GetUserByIdentityNameAsync));

        var normalizedIdentityName = this.MemberHelper.NormalizeIdentityName(dto.IdentityName);

        var res = await _userAccountRepository.QueryOneAsync(x => x.IdentityName == normalizedIdentityName);

        return res;
    }

    public virtual async Task<bool> IsUserIdentityNameExistAsync(IdentityNameDto dto)
    {
        if (string.IsNullOrWhiteSpace(dto?.IdentityName))
            throw new ArgumentNullException(nameof(GetUserByIdentityNameAsync));

        var normalizedIdentityName = this.MemberHelper.NormalizeIdentityName(dto.IdentityName);

        var db = await this._userAccountRepository.GetDbContextAsync();

        var res = await db.Set<SysUser>().IgnoreQueryFilters().AnyAsync(x => x.IdentityName == normalizedIdentityName);

        return res;
    }

    public async Task<SysUserDto> GetUserDtoByIdAsync(IdDto dto)
    {
        if (string.IsNullOrWhiteSpace(dto?.Id))
            throw new ArgumentNullException(nameof(GetUserDtoByIdAsync));

        var entity = await _userAccountRepository.QueryOneAsync(x => x.Id == dto.Id);

        if (entity == null)
            return null;

        return this.ObjectMapper.Map<SysUser, SysUserDto>(entity);
    }

    public async Task<SysUserDto> GetUserDtoByIdAsync(IdDto filter, CachePolicy cachePolicyOption)
    {
        var option = new CacheOption<SysUserDto>($"user.{filter.Id}.dto");

        var data = await this.CacheProvider.ExecuteWithPolicyAsync(
            () => GetUserDtoByIdAsync(filter), option, cachePolicyOption);

        return data;
    }
}