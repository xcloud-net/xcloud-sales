using Microsoft.Extensions.Configuration;
using XCloud.Core.Application.WorkContext;
using XCloud.Core.Configuration;

namespace XCloud.Core;

public static class CoreExtensions
{
    public static AppConfig ResolveAppConfig(this IServiceProvider serviceProvider) => serviceProvider.GetRequiredService<AppConfig>();

    public static bool TryGetNonEmptyString(this IConfiguration config, string key, out string val)
    {
        if (config == null)
            throw new ArgumentNullException(nameof(config));

        if (string.IsNullOrWhiteSpace(key))
            throw new ArgumentNullException(nameof(key));
        
        val = config[key];

        return !string.IsNullOrWhiteSpace(val);
    }

    public static bool InitDatabaseRequired(this IConfiguration config)
    {
        var res = config["app:config:init_db"]?.ToBool() ?? false;
        return res;
    }
}