using System.Threading.Tasks;
using XCloud.Core.DependencyInjection.Extension;
using XCloud.Core.DependencyInjection;

namespace XCloud.Core.Development.Builtin;

public class ConfigDevelopmentInformationProvider : IDevelopmentInformationProvider, IAutoRegistered
{
    private readonly IServiceProvider serviceProvider;
    public ConfigDevelopmentInformationProvider(IServiceProvider serviceProvider)
    {
        this.serviceProvider = serviceProvider;
    }

    public string ProviderName => "配置";

    public async Task<object> Information()
    {
        await Task.CompletedTask;

        var config = this.serviceProvider.ResolveConfiguration();
        var dict = config.ConfigAsKV();
        var pairs = dict.OrderBy(x => x.Key).Select(x => $"{x.Key}={x.Value}").ToArray();

        return pairs;
    }
}