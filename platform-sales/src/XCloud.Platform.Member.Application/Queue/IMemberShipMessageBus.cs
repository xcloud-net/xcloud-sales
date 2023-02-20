using System.Threading.Tasks;
using DotNetCore.CAP;
using Volo.Abp.DependencyInjection;
using XCloud.Core.Dto;
using XCloud.Platform.Member.Application.Service.User;

namespace XCloud.Platform.Member.Application.Queue;

public interface IMemberShipMessageBus
{
    Task NotifyRefreshUserInfoAsync(string userId);
    Task SendSms(UserPhoneBindSmsMessage message);
}

[ExposeServices(typeof(IMemberShipMessageBus))]
public class MemberShipMessageBus : IMemberShipMessageBus, ITransientDependency
{
    private readonly ICapPublisher _capPublisher;
    public MemberShipMessageBus(ICapPublisher capPublisher)
    {
        this._capPublisher = capPublisher;
    }

    public async Task NotifyRefreshUserInfoAsync(string userId)
    {
        if(string.IsNullOrWhiteSpace(userId))
            return;
        await this._capPublisher.PublishAsync(MemberMessageTopics.RefreshUserInfo, new IdDto(userId));
    }

    public async Task SendSms(UserPhoneBindSmsMessage message)
    {
        await Task.CompletedTask;
    }
}