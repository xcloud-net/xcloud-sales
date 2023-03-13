using Microsoft.Extensions.DependencyInjection;
using Volo.Abp.Modularity;

namespace XCloud.Platform.Shared.Configuration;

public static class OptionsExtension
{
    public static void ConfigPlatformSharedOptions(this ServiceConfigurationContext context)
    {
        var configuration = context.Services.GetConfiguration();
    }
}