using System;
using Microsoft.Extensions.Configuration;
using Volo.Abp.DependencyInjection;

namespace XCloud.Platform.Auth.IdentityServer;

[ExposeServices(typeof(IdentityServerAuthConfig))]
public class IdentityServerAuthConfig : IScopedDependency
{
    private readonly IConfiguration _config;

    public IdentityServerAuthConfig(IConfiguration configuration)
    {
        this._config = configuration.GetSection("app")?.GetSection("identity_server");
        if (this._config == null)
            throw new ArgumentNullException(nameof(configuration));
    }

    public string ClientId => this._config["client_id"];

    public string ClientSecret => this._config["client_secret"];

    public string Scope => this._config["scope"];
}