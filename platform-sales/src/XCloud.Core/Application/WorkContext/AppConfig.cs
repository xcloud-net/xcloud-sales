using System.Reflection;
using System.Text;
using Microsoft.Extensions.Configuration;
using Volo.Abp.DependencyInjection;
using XCloud.Core.DependencyInjection.Extension;
using XCloud.Core.Builder;

namespace XCloud.Core.Application.WorkContext;

[ExposeServices(typeof(AppConfig))]
public class AppConfig : ISingletonDependency
{
    private readonly IServiceProvider serviceProvider;
    private readonly EntryModuleWrapper entryModuleWrapper;
    private readonly IConfiguration configuration;

    public AppConfig(IServiceProvider serviceProvider)
    {
        this.serviceProvider = serviceProvider;
        this.entryModuleWrapper = serviceProvider.GetRequiredService<EntryModuleWrapper>();
        this.configuration = serviceProvider.ResolveConfiguration();
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

    public string AppName() => GetAppName(this.configuration, this.entryModuleWrapper.EntryAssembly);

    public Encoding Encoding => Encoding.UTF8;
}