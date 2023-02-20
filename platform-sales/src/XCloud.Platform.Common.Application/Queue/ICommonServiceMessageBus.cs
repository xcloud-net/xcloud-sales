using Volo.Abp.DependencyInjection;

namespace XCloud.Platform.Common.Application.Queue;

public interface ICommonServiceMessageBus
{
    //
}

[ExposeServices(typeof(ICommonServiceMessageBus))]
public class CommonServiceMessageBus : ICommonServiceMessageBus, ITransientDependency
{
    //
}