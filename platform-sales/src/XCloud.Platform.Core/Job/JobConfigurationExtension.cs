using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using XCloud.Core;

namespace XCloud.Platform.Core.Job;

public static class JobConfigurationExtension
{
    public static bool AutoStartPlatformJob(this IServiceProvider serviceProvider)
    {
        using var s = serviceProvider.CreateScope();
        
        var option = s.ServiceProvider.GetRequiredService<IOptions<PlatformJobOption>>().Value;
        if (option == null)
            throw new ConfigException(nameof(AutoStartPlatformJob));
        
        return option.AutoStartJob;
    }
}