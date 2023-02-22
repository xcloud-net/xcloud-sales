using Microsoft.AspNetCore.Mvc;
using Volo.Abp.Authorization;
using Volo.Abp.Uow;
using XCloud.Core.Cache;
using XCloud.Core.Dto;
using XCloud.Sales.Service.Authentication;
using XCloud.Sales.Service.Users;

namespace XCloud.Sales.Mall.Api.Controller.User;

[Route("api/mall/user")]
public class UserController : ShopBaseController
{
    private readonly IUserService _userService;
    private readonly IUserPointService _userPointService;
    private readonly IUserBalanceService _userBalanceService;
    private readonly IUserProfileService _userProfileService;
    
    public UserController(
        IUserService userService,
        IUserPointService userPointService,
        IUserBalanceService userBalanceService,
        IUserProfileService userProfileService)
    {
        this._userBalanceService = userBalanceService;
        this._userService = userService;
        this._userPointService = userPointService;
        this._userProfileService = userProfileService;
    }

    [HttpPost("sync-profile-from-platform")]
    public async Task<ApiResponse<object>> SyncProfileFromPlatformAsync()
    {
        var loginUser = await this.StoreAuthService.GetRequiredStoreUserAsync();

        await this._userProfileService.UpdateProfileFromPlatformAsync(loginUser.Id);

        return new ApiResponse<object>();
    }

    [HttpPost("profile")]
    [UnitOfWork(IsDisabled = true)]
    public async Task<ApiResponse<StoreUserDto>> QueryUserProfileAsync()
    {
        var loginUser = await this.StoreAuthService.GetRequiredStoreUserAsync();

        //try set last activity time
        await this.EventBusService.NotifySetUserLastActivityTimeAsync(loginUser.Id);
            
        var model = await this._userProfileService.QueryProfileAsync(loginUser.Id,
            new CachePolicy() { Cache = true });
            
        if (model == null)
            throw new AbpAuthorizationException(nameof(model));

        return new ApiResponse<StoreUserDto>(model);
    }

    [HttpPost("balance-and-points")]
    public async Task<ApiResponse<UserBalanceAndPoints>> UserBalanceAndPointsAsync()
    {
        var loginUser = await this.StoreAuthService.GetRequiredStoreUserAsync();

        var user = await this._userService.GetUserByIdAsync(loginUser.Id);

        if (user == null)
            throw new EntityNotFoundException();

        return new ApiResponse<UserBalanceAndPoints>(new UserBalanceAndPoints(user));
    }

    [HttpPost("points-history")]
    public async Task<PagedResponse<PointsHistoryDto>> QueryPointsPagingAsync([FromBody] QueryPointsInput dto)
    {
        var loginUser = await this.StoreAuthService.GetRequiredStoreUserAsync();
        dto.UserId = loginUser.Id;

        var response = await this._userPointService.QueryPagingAsync(dto);

        return response;
    }

    [HttpPost("balance-history")]
    public async Task<PagedResponse<BalanceHistoryDto>> QueryBalancePagingAsync([FromBody] QueryBalanceInput dto)
    {
        var loginUser = await this.StoreAuthService.GetRequiredStoreUserAsync();
        dto.UserId = loginUser.Id;

        var response = await this._userBalanceService.QueryPagingAsync(dto);

        return response;
    }
}