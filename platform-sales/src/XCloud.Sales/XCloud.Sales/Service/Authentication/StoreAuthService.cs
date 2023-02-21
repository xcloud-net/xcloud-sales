using Microsoft.AspNetCore.Http;
using Microsoft.Net.Http.Headers;
using XCloud.Core.Dto;
using XCloud.Platform.Member.Application.Service.Admin;
using XCloud.Platform.Member.Application.Service.User;
using XCloud.Redis;
using XCloud.Sales.Application;
using XCloud.Sales.Clients.Platform;
using XCloud.Sales.Data.Domain.Users;
using XCloud.Sales.Framework;
using XCloud.Sales.Service.Stores;
using XCloud.Sales.Service.Users;

namespace XCloud.Sales.Service.Authentication;

public interface IStoreAuthService : ISalesAppService
{
    Task<AuthedStoreAdministratorResult> GetStoreAdministratorAsync();

    Task<AuthedStoreUserResult> GetStoreUserAsync();

    Task<AuthedStoreManagerResult> GetStoreManagerAsync(string storeId = null);
}

[ExposeServices(typeof(IStoreAuthService))]
public class StoreAuthService : SalesAppService, IStoreAuthService
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    private readonly PlatformPublicService _userClientService;
    private readonly ISalesWorkContext _salesWorkContext;
    private readonly IUserService _userService;
    private readonly PlatformInternalService _platformInternalService;
    private readonly Users.IUserProfileService _userProfileService;
    private readonly IStoreManagerService _storeManagerService;

    public StoreAuthService(
        IStoreManagerService storeManagerService,
        Users.IUserProfileService userProfileService,
        ISalesWorkContext salesWorkContext,
        IHttpContextAccessor httpContextAccessor,
        IUserService userService,
        PlatformPublicService userClientService,
        PlatformInternalService platformInternalService)
    {
        this._storeManagerService = storeManagerService;
        this._platformInternalService = platformInternalService;
        this._userService = userService;
        this._salesWorkContext = salesWorkContext;
        this._httpContextAccessor = httpContextAccessor;

        this._userProfileService = userProfileService;

        this._userClientService = userClientService;
    }

    private TimeSpan CacheExpired => TimeSpan.FromMinutes(2);

    private string GetBearerToken()
    {
        if (this._httpContextAccessor.HttpContext == null)
            throw new NotSupportedException(nameof(this._httpContextAccessor));

        var hasTokenHeader =
            this._httpContextAccessor.HttpContext.Request.Headers.TryGetValue(HeaderNames.Authorization,
                out var val);
        if (hasTokenHeader)
        {
            var headerValue = (string)val;
            if (!string.IsNullOrWhiteSpace(headerValue))
            {
                var token = headerValue.Split(' ').Skip(1).FirstOrDefault();
                return token;
            }
        }

        return string.Empty;
    }

    async Task<AuthedGlobalUserResult> GetGlobalAuthedUserAsync()
    {
        var cachedResult = this._salesWorkContext.AuthedGlobalUser;
        if (cachedResult != null)
            return cachedResult;

        var res = new AuthedGlobalUserResult();
        try
        {
            var token = this.GetBearerToken();
            if (string.IsNullOrWhiteSpace(token))
            {
                res.TokenIsRequired = true;
                res.SetError("token is required");
                return res;
            }

            var key = $"sales.global_user.by.token:{token}";

            var model = await this.CacheProvider.GetOrSetAsync(
                () => this._userClientService.GetCurrentLoginUserAsync(token),
                new CacheOption<ApiResponse<SysUserDto>>(key, this.CacheExpired));

            if (model == null || !model.IsSuccess() || model.Data == null)
            {
                res.GlobalUserIsNotValid = true;
                res.SetError($"global user is not valid,err:{model?.Error?.Message}");
                return res;
            }

            var globalUser = model.Data;

            res.SetData(globalUser);
            return res;
        }
        finally
        {
            this._salesWorkContext.AuthedGlobalUser = res;
        }
    }

    public virtual async Task<AuthedStoreAdministratorResult> GetStoreAdministratorAsync()
    {
        var cachedResult = this._salesWorkContext.AuthedStoreAdministrator;
        if (cachedResult != null)
            return cachedResult;

        var res = new AuthedStoreAdministratorResult();

        try
        {
            var globalUserResponse = await this.GetGlobalAuthedUserAsync();
            if (!globalUserResponse.IsSuccess())
            {
                res.SetError(globalUserResponse.Error.Message);
                return res;
            }
            var globalUser = globalUserResponse.Data!;

            var key = $"sales.admin.by.user:{globalUser.UserId}";

            var data = await this.CacheProvider.GetOrSetAsync(
                () => this._platformInternalService.AdminAuthAsync(globalUserResponse.Data),
                new CacheOption<ApiResponse<SysAdminDto>>(key, this.CacheExpired));

            if (data == null || !data.IsSuccess())
            {
                res.SetError(data?.Error?.Message);
                return res;
            }

            res.SetData(new StoreAdministrator(data.Data.AdminId));

            return res;
        }
        finally
        {
            this._salesWorkContext.AuthedStoreAdministrator = res;
        }
    }

    private async Task<User> TryCreateMallUserAsync(string globalUserId)
    {
        if (string.IsNullOrWhiteSpace(globalUserId))
            throw new ArgumentNullException(nameof(globalUserId));

        using var lk = await this.RedLockClient.RedLockFactory.CreateLockAsync(
            resource:
            $"create-mall-user-for-global-user:{globalUserId}",
            expiryTime: TimeSpan.FromSeconds(5));
            
        if (lk.IsAcquired)
        {
            var mallUser = await this._userService.GetOrCreateUserByGlobalUserId(globalUserId);

            //update profile from platform
            await this._userProfileService.UpdateProfileFromPlatformAsync(mallUser.Id);

            //todo:notify worker to update sales user basic profile
            //update:invitation info,nick name avatar gender etc...

            return mallUser;
        }
        else
        {
            throw new FailToGetRedLockException($"error to get lock when create sales user");
        }
    }

    public async Task<AuthedStoreUserResult> GetStoreUserAsync()
    {
        var cachedResult = this._salesWorkContext.AuthedStoreUser;
        if (cachedResult != null)
            return cachedResult;

        var res = new AuthedStoreUserResult();
        try
        {
            var globalUserResponse = await this.GetGlobalAuthedUserAsync();
            if (!globalUserResponse.IsSuccess())
            {
                res.GlobalUserIsNotValid = true;
                res.SetError(globalUserResponse.Error.Message);
                return res;
            }

            var mallUser = await this._userService.GetUserByGlobalUserIdAsync(
                globalUserResponse.Data.UserId,
                new CachePolicy() { Cache = true });

            if (mallUser == null)
            {
                mallUser = await this.TryCreateMallUserAsync(globalUserResponse.Data.UserId);
            }

            if (mallUser.IsDeleted || !mallUser.Active)
            {
                res.StoreUserIsNotValid = true;
                res.SetError("sales user is not valid");
                return res;
            }

            res.SetData(mallUser);
            return res;
        }
        finally
        {
            this._salesWorkContext.AuthedStoreUser = res;
        }
    }

    public async Task<AuthedStoreManagerResult> GetStoreManagerAsync(string storeId = null)
    {
        var cachedResult = this._salesWorkContext.AuthedStoreManager;
        if (cachedResult != null)
            return cachedResult;

        var res = new AuthedStoreManagerResult();
        try
        {
            if (string.IsNullOrWhiteSpace(storeId))
                storeId = await this.CurrentStoreSelector.GetStoreIdOrEmptyAsync();
            if (string.IsNullOrWhiteSpace(storeId))
            {
                res.SelectStoreRequired = true;
                res.SetError("please select store");
                return res;
            }

            var globalUserResponse = await this.GetGlobalAuthedUserAsync();
            if (!globalUserResponse.IsSuccess())
            {
                res.SetError(globalUserResponse.Error.Message);
                return res;
            }

            var storeManager = await this._storeManagerService.QueryByGlobalUserIdAsync(
                storeId,
                globalUserResponse.Data.UserId, 
                new CachePolicy() { Cache = true });

            if (storeManager == null)
            {
                res.NotStoreManager = true;
                res.SetError("not store manager");
                return res;
            }

            res.SetData(storeManager);

            return res;
        }
        finally
        {
            this._salesWorkContext.AuthedStoreManager = res;
        }
    }
}