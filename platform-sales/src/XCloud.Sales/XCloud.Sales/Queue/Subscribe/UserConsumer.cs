using DotNetCore.CAP;
using XCloud.Core.Dto;
using XCloud.Sales.Application;
using XCloud.Sales.Service.Users;

namespace XCloud.Sales.Queue.Subscribe;

[UnitOfWork]
public class UserConsumer : SalesAppService, ICapSubscribe
{
    private readonly IUserService _userService;
    private readonly IUserProfileService _userProfileService;

    public UserConsumer(IUserService userService,
        IUserProfileService userProfileService)
    {
        this._userProfileService = userProfileService;
        this._userService = userService;
    }

    [CapSubscribe(SalesMessageTopics.SyncUserInfoFromPlatform)]
    public virtual async Task SyncUserInfoFromPlatform(IdDto<int> message)
    {
        if (message == null)
            return;

        await this._userProfileService.UpdateProfileFromPlatformAsync(message.Id);
        await this.EventBusService.NotifyRefreshUserInfoAsync(message.Id);
    }
    
    [CapSubscribe(SalesMessageTopics.RefreshUserInformation)]
    public virtual async Task RefreshUserInformation(IdDto<int> message)
    {
        if (message == null)
            return;
        
        await this._userProfileService.QueryProfileAsync(message.Id, new CachePolicy() { Refresh = true });
    }

    [CapSubscribe(SalesMessageTopics.UpdateUserLastActivityTime)]
    public virtual async Task UpdateUserLastActivityTime(IdDto<int> message)
    {
        if (message == null)
            return;

        await this._userService.TrySetLastActivityTimeAsync(message.Id);
    }
}