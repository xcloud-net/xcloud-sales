using System.Threading.Tasks;
using XCloud.Core.DependencyInjection;
using XCloud.Core.DependencyInjection.ServiceWrapper;

namespace XCloud.Core.Development.Builtin;

public class SingleInstanceDevelopmentInformationProvider : IDevelopmentInformationProvider, IAutoRegistered
{
    private readonly IServiceProvider serviceProvider;
    public SingleInstanceDevelopmentInformationProvider(IServiceProvider serviceProvider)
    {
        this.serviceProvider = serviceProvider;
    }

    public string ProviderName => "单例对象";

    public async Task<object> Information()
    {
        await Task.CompletedTask;

        var components = this.serviceProvider.GetServices<ISingleInstanceService>().ToArray();
        var names = components.Select(x => x.GetType().FullName).ToArray();

        return names;
    }
}