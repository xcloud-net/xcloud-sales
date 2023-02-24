using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using XCloud.Core;
using XCloud.Platform.Core.Database;

namespace XCloud.Platform.Core;

public static class PlatformCoreExtension
{
    public static bool AutoCreatePlatformDatabase(this IServiceProvider serviceProvider)
    {
        using var s = serviceProvider.CreateScope();

        var option = s.ServiceProvider.GetRequiredService<IOptions<PlatformDatabaseOption>>()?.Value;

        if (option == null)
            throw new ConfigException(nameof(AutoCreatePlatformDatabase));

        return option.AutoCreateDatabase;
    }

}