using System;
using System.Collections.Generic;
using System.Linq;
using IdentityServer4.Models;
using XCloud.Core.Extension;
using XCloud.Platform.Shared;
using XCloud.Platform.Shared.Constants;
using static IdentityServer4.IdentityServerConstants;

namespace XCloud.Platform.Auth.IdentityServer.Database;

/// <summary>
/// https://identityserver4.readthedocs.io/en/release/reference/identity_resource.html
/// </summary>
public static class IdentityServerStaticConfig
{
    public static List<ApiScope> TestApiScopes()
    {
        return new List<ApiScope>()
        {
            new ApiScope("water", "water")
        };
    }

    public static List<ApiResource> TestApiResource()
    {
        var secret = "456".Sha256();
        return new List<ApiResource>()
        {
            new ApiResource("water", "water")
            {
                ApiSecrets = new Secret[]
                {
                    new Secret(secret)
                },
                Scopes = new[] { "water" }
            }
        };
    }

    public static List<IdentityResource> TestIdentityResource()
    {
        return new List<IdentityResource>()
        {
            new IdentityResources.OpenId(),
            new IdentityResources.Profile(),
            new IdentityResources.Phone(),
            new IdentityResources.Email(),
            new IdentityResources.Address(),
            new IdentityResource("relation-ship", new string[] { "relation" }),
        };
    }

    public static List<Client> TestClients()
    {
        var scopes = new string[]
        {
            StandardScopes.OfflineAccess,
            StandardScopes.OpenId,
            StandardScopes.Profile,
            "water"
        };

        var secret = "123".Sha256();
        var list = new List<Client>()
        {
            new Client()
            {
                ClientId = "wx-jwt",
                AccessTokenType = AccessTokenType.Jwt,
                RequireClientSecret = false,
                AccessTokenLifetime = (int)TimeSpan.FromDays(30).TotalSeconds,
                AllowedGrantTypes = new string[]
                {
                    GrantType.AuthorizationCode,
                    GrantType.ResourceOwnerPassword,
                }
            },
            new Client()
            {
                ClientId = "wx-code",
                AccessTokenType = AccessTokenType.Reference,
                RequireClientSecret = false,
                AccessTokenLifetime = (int)TimeSpan.FromDays(30).TotalSeconds,
                AllowedGrantTypes = new string[]
                {
                    GrantType.AuthorizationCode,
                    GrantType.ResourceOwnerPassword,
                }
            },
            new Client()
            {
                ClientId = "wx-imp",
                AccessTokenType = AccessTokenType.Reference,
                AccessTokenLifetime = (int)TimeSpan.FromDays(30).TotalSeconds,
                AllowedGrantTypes = new string[]
                {
                    GrantType.Implicit,
                    GrantType.ResourceOwnerPassword,
                },
            }
        };
        foreach (var m in list)
        {
            m.AllowedScopes = scopes;
            m.ClientSecrets = new[]
            {
                new Secret(secret)
            };
            m.AllowedGrantTypes = m.AllowedGrantTypes.AppendManyItems(new string[]
            {
                IdentityConsts.GrantType.UserWechat,
                IdentityConsts.GrantType.AdminPassword,
                IdentityConsts.GrantType.InternalGrantType
            }).ToArray();
            //m.AllowedCorsOrigins = new string[] { };
            m.AllowAccessTokensViaBrowser = true;
            m.AllowOfflineAccess = true;
            m.AllowPlainTextPkce = true;
            m.AllowRememberConsent = true;
            m.RedirectUris = new string[]
            {
                "https://www.baidu.com"
            };
            m.PostLogoutRedirectUris = new string[]
            {
                "https://www.baidu.com"
            };
        }

        return list;
    }
}