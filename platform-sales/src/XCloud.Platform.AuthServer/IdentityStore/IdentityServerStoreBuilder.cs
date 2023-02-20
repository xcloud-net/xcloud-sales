﻿using IdentityServer4.Stores;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using XCloud.Core.DependencyInjection.Extension;
using XCloud.Database.EntityFrameworkCore.MySQL;
using XCloud.Platform.AuthServer.IdentityStore.IdsDbContext;
using XCloud.Platform.AuthServer.IdentityStore.IdsStores;

namespace XCloud.Platform.AuthServer.IdentityStore;

public static class IdentityServerStoreBuilder
{
    /// <summary>
    /// 使用写死的配置数据
    /// </summary>
    /// <param name="identityBuilder"></param>
    /// <returns></returns>
    public static IIdentityServerBuilder AddConfigurationStore(this IIdentityServerBuilder identityBuilder)
    {
        //identityBuilder.AddInMemoryApiScopes(IdentityConfig.TestApiScopes());
        //identityBuilder.AddInMemoryApiResources(IdentityConfig.TestApiResource());
        //identityBuilder.AddInMemoryIdentityResources(IdentityConfig.TestIdentityResource());
        //identityBuilder.AddInMemoryClients(IdentityConfig.TestClients());

        identityBuilder.Services.RemoveAll<IClientStore>().AddTransient<IClientStore, ClientStore>();
        identityBuilder.Services.RemoveAll<IResourceStore>().AddTransient<IResourceStore, ResourceStore>();

        return identityBuilder;
    }

    /// <summary>
    /// 授权存储
    /// </summary>
    /// <param name="identityServerBuilder"></param>
    /// <returns></returns>
    public static IIdentityServerBuilder AddOperationStore(this IIdentityServerBuilder identityServerBuilder)
    {
        identityServerBuilder.AddOperationalStore<IdentityOperationDbContext>(option =>
        {
            option.ResolveDbContextOptions = (provider, builder) =>
            {
                var configuration = provider.ResolveConfiguration();

                builder.UseMySqlProvider(configuration, "OAuthGrants");
            };
            //自动清理无用token
            option.EnableTokenCleanup = true;
        });

        //builder.Services.RemoveAll<IPersistedGrantStore>();
        //builder.AddPersistedGrantStore<IdsStores.PersistedGrantStore>();

        return identityServerBuilder;
    }
}