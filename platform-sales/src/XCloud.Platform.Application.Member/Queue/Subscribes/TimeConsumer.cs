using System.Threading.Tasks;
using DotNetCore.CAP;
using Volo.Abp.DependencyInjection;
using XCloud.Platform.Core.Application;

namespace XCloud.Platform.Application.Member.Queue.Subscribes;

[UnitOfWork]
public class TestCap : PlatformApplicationService, ICapSubscribe, ITransientDependency
{
    [CapSubscribe(MemberMessageTopics.SendSms)]
    public virtual async Task XX(string data)
    {
        await Task.CompletedTask;
    }
}