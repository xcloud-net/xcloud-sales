using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Volo.Abp.Modularity;
using XCloud.Platform.Shared.Settings;

namespace XCloud.Platform.Shared.Configuration;

public static class OptionsExtension
{
    public static void ConfigPlatformSharedOptions(this ServiceConfigurationContext context)
    {
        var configuration = context.Services.GetConfiguration();

        context.Services.Configure<PlatformServiceAddressOption>(
            configuration.GetRequiredSection("PlatformServiceAddress"));
    }
}