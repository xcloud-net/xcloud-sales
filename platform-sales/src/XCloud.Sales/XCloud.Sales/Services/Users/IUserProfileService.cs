using XCloud.Sales.Clients.Platform;
using XCloud.Sales.Data;
using XCloud.Sales.Data.Domain.Users;

namespace XCloud.Sales.Services.Users;

public interface IUserProfileService : ISalesAppService
{
    Task UpdateProfileFromPlatformAsync(int userId);
    
    Task SetNickNameAsync(int userId, string nickName);
    
    Task SetAvatarAsync(int userId, string avatar);
    
    Task<StoreUserDto> QueryProfileAsync(int userId);
    
    Task<StoreUserDto> QueryProfileAsync(int userId, CachePolicy cachePolicyOption);
}

public class UserProfileService : SalesAppService, IUserProfileService
{
    private readonly ISalesRepository<User> _userRepository;
    private readonly IUserGradeService _userGradeService;
    private readonly PlatformInternalService _platformInternalService;

    public UserProfileService(
        PlatformInternalService platformInternalService,
        ISalesRepository<User> userRepository,
        IUserGradeService userGradeService)
    {
        this._platformInternalService = platformInternalService;
        this._userRepository = userRepository;
        this._userGradeService = userGradeService;
    }

    public async Task UpdateProfileFromPlatformAsync(int userId)
    {
        if (userId <= 0)
            throw new ArgumentNullException(nameof(userId));

        var user = await this._userRepository.QueryOneAsync(x => x.Id == userId);
        if (user == null)
            throw new EntityNotFoundException(nameof(UpdateProfileFromPlatformAsync));
            
        if (string.IsNullOrWhiteSpace(user.GlobalUserId))
            return;

        var platformUserProfile = await this._platformInternalService.QueryProfileByUserIdAsync(user.GlobalUserId);
        if (platformUserProfile == null)
            return;

        user.NickName = platformUserProfile.NickName;
        user.Avatar = platformUserProfile.Avatar;
        user.AccountMobile = platformUserProfile.AccountMobile;
        user.LastModificationTime = this.Clock.Now;

        await this._userRepository.UpdateAsync(user);
    }

    public async Task SetNickNameAsync(int userId, string nickName)
    {
        if (userId <= 0)
            throw new ArgumentNullException(nameof(userId));

        var user = await this._userRepository.QueryOneAsync(x => x.Id == userId);

        if (user == null)
            throw new EntityNotFoundException(nameof(SetAvatarAsync));

        user.NickName = nickName;
        user.LastModificationTime = this.Clock.Now;

        await this._userRepository.UpdateAsync(user);
    }

    public async Task SetAvatarAsync(int userId, string avatar)
    {
        if (userId <= 0)
            throw new ArgumentNullException(nameof(userId));

        var user = await this._userRepository.QueryOneAsync(x => x.Id == userId);

        if (user == null)
            throw new EntityNotFoundException(nameof(SetAvatarAsync));

        user.Avatar = avatar;
        user.LastModificationTime = this.Clock.Now;

        await this._userRepository.UpdateAsync(user);
    }

    public async Task<StoreUserDto> QueryProfileAsync(int userId,CachePolicy cachePolicyOption)
    {
        var key = $"mall-user-profile-user:{userId}";
        var mallUser = await this.CacheProvider.ExecuteWithPolicyAsync(() => this.QueryProfileAsync(userId),
            new CacheOption<StoreUserDto>(key, TimeSpan.FromMinutes(3)),
            cachePolicyOption);
        return mallUser;
    }

    public async Task<StoreUserDto> QueryProfileAsync(int userId)
    {
        var db = await this._userRepository.GetDbContextAsync();

        var query = db.Set<User>().AsNoTracking();

        var data = await query.FirstOrDefaultAsync(x => x.Id == userId);

        if (data == null)
            return null;

        var dto = this.ObjectMapper.Map<User, StoreUserDto>(data);

        var grade = await this._userGradeService.GetGradeByUserIdAsync(dto.Id);
        dto.Grade = grade;
        if (grade != null)
        {
            dto.GradeName = grade.Name;
            dto.GradeDescription = grade.Description;
        }

        if (!string.IsNullOrWhiteSpace(dto.GlobalUserId))
        {
            var sysUser = await this._platformInternalService.QueryProfileByUserIdAsync(dto.GlobalUserId);
            dto.SysUser = sysUser;
        }

        return dto;
    }
}