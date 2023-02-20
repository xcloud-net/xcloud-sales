using XCloud.Core.Dto;
using XCloud.Logging;
using XCloud.Sales.Services.Users;

namespace XCloud.Sales.Job;

[LogExceptionSilence]
[UnitOfWork]
public class UserJobs : SalesAppService, ITransientDependency
{
    private readonly IUserService _userService;

    public UserJobs(IUserService userService)
    {
        this._userService = userService;
    }

    [LogExceptionSilence]
    public virtual async Task TriggerSyncUserInformationFromPlatformAsync()
    {
        var page = 0;
        while (true)
        {
            ++page;
            var dto = new PagedRequest()
            {
                Page = page,
                PageSize = 100,
                SkipCalculateTotalCount = true
            };
            var response = await this._userService.QuerySimplePagingAsync(dto);

            if(!response.Any())
                break;

            foreach (var m in response)
            {
                await this.EventBusService.NotifySyncUserInfoFromPlatformAsync(m.Id);
            }
        }
    }
}