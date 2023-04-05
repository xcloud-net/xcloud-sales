using System.Threading.Tasks;
using DotNetCore.CAP;
using XCloud.Core.Dto;
using XCloud.Platform.Application.Member.Service.User;
using XCloud.Platform.Core.Application;

namespace XCloud.Platform.Application.Member.Queue;

public class MemberMessageBus : PlatformApplicationService
{
    private readonly ICapPublisher _capPublisher;

    public MemberMessageBus(ICapPublisher capPublisher)
    {
        this._capPublisher = capPublisher;
    }

    public async Task NotifyRefreshUserInfoAsync(string userId)
    {
        if (string.IsNullOrWhiteSpace(userId))
            return;
        await this._capPublisher.PublishAsync(MemberMessageTopics.RefreshUserInfo, new IdDto(userId));
    }

    public async Task SendSms(UserPhoneBindSmsMessage message)
    {
        await Task.CompletedTask;
    }
}