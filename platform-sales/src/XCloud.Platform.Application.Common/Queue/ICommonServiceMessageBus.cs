using Volo.Abp.DependencyInjection;

namespace XCloud.Platform.Application.Common.Queue;

public interface ICommonServiceMessageBus
{
    //
}

[ExposeServices(typeof(ICommonServiceMessageBus))]
public class CommonServiceMessageBus : ICommonServiceMessageBus, ITransientDependency
{
    //
}