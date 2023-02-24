using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using XCloud.Core;

namespace XCloud.Platform.AuthServer;

public static class AuthServerExtension
{
    public static bool AutoCreateAuthServerDatabase(this IServiceProvider serviceProvider)
    {
        var option = serviceProvider.GetRequiredService<IOptions<AuthServerDatabaseOption>>().Value;

        if (option == null)
            throw new ConfigException(nameof(AutoCreateAuthServerDatabase));

        return option.AutoCreateDatabase;
    }
}