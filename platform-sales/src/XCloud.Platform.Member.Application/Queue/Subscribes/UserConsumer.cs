using System.Threading.Tasks;
using DotNetCore.CAP;
using Volo.Abp.DependencyInjection;
using XCloud.Core.Dto;
using XCloud.Platform.Core.Application;
using XCloud.Platform.Member.Application.Service.User;

namespace XCloud.Platform.Member.Application.Queue.Subscribes;

[UnitOfWork]
public class UserConsumer : PlatformApplicationService, ITransientDependency
{
    private readonly IUserProfileService _userProfileService;

    public UserConsumer(IUserProfileService userProfileService)
    {
        this._userProfileService = userProfileService;
    }

    [CapSubscribe(MemberMessageTopics.RefreshUserInfo)]
    public virtual async Task RefreshSysUserInfoAsync(IdDto dto)
    {
        await this._userProfileService.QueryProfileByUserIdAsync(dto, new CachePolicy() { Refresh = true });
    }
}