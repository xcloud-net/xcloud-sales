using System.Reflection;
using System.Text;
using Microsoft.Extensions.Configuration;
using Volo.Abp.DependencyInjection;
using XCloud.Core.Configuration.Builder;
using XCloud.Core.DependencyInjection.Extension;

namespace XCloud.Core.Configuration;

[ExposeServices(typeof(AppConfig))]
public class AppConfig : ISingletonDependency
{
    private readonly IServiceProvider _serviceProvider;
    private readonly EntryModuleWrapper _entryModuleWrapper;
    private readonly IConfiguration _configuration;

    public AppConfig(IServiceProvider serviceProvider)
    {
        this._serviceProvider = serviceProvider;
        this._entryModuleWrapper = serviceProvider.GetRequiredService<EntryModuleWrapper>();
        this._configuration = serviceProvider.ResolveConfiguration();
    }

    public static string GetAppName(IConfiguration configuration, Assembly entryAssembly)
    {
        if (configuration.TryGetNonEmptyString("app:name", out var name))
        {
            return name;
        }

        if (configuration.TryGetNonEmptyString("app_name", out var name1))
        {
            return name1;
        }

        var entryAssemblyName = entryAssembly?.GetName()?.Name;
        if (!string.IsNullOrWhiteSpace(entryAssemblyName))
        {
            return entryAssemblyName;
        }

        return "unknowApp";
    }

    public string AppName() => GetAppName(this._configuration, this._entryModuleWrapper.EntryAssembly);

    public Encoding Encoding => Encoding.UTF8;
}