using Microsoft.Extensions.Configuration;
using XCloud.Core;
using XCloud.Platform.Auth.Settings;

namespace XCloud.Platform.Auth.Configuration;

public static class OAuthServerExtension
{
    public static OAuthServerOption GetOAuthServerOption(this IConfiguration configuration)
    {
        var section = configuration.GetSection("OAuthServer");
        if (!section.Exists())
            throw new ConfigException(nameof(GetOAuthServerOption));

        var option = new OAuthServerOption();

        section.Bind(option);
        
        return option;
    }
}