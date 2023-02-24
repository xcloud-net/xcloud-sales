// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.


using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IdentityServer4.Models;
using IdentityServer4.Stores;
using XCloud.Core.Application.WorkContext;

namespace XCloud.Platform.AuthServer.IdentityServer.PersistentStore.InMemory;

/// <summary>
/// Implementation of IResourceStore thats uses EF.
/// </summary>
/// <seealso cref="IdentityServer4.Stores.IResourceStore" />
public class ResourceStore : IResourceStore
{
    private readonly IWorkContext _context;

    public ResourceStore(IWorkContext<ResourceStore> context)
    {
        this._context = context;
    }

    public async Task<IEnumerable<ApiResource>> FindApiResourcesByNameAsync(IEnumerable<string> apiResourceNames)
    {
        if(apiResourceNames==null)
            throw new ArgumentNullException(nameof(apiResourceNames));
        
        await Task.CompletedTask;

        var apiResourceNamesArray = apiResourceNames.ToArray();

        var res = IdentityServerStaticConfig.TestApiResource().Where(x => apiResourceNamesArray.Contains(x.Name)).ToArray();

        return res;
    }

    public async Task<IEnumerable<ApiResource>> FindApiResourcesByScopeNameAsync(IEnumerable<string> scopeNames)
    {
        if (scopeNames == null)
            throw new ArgumentNullException(nameof(scopeNames));
        
        await Task.CompletedTask;

        var scopeNamesArray = scopeNames.ToArray();

        var res = IdentityServerStaticConfig.TestApiResource().Where(x => scopeNamesArray.Intersect(x.Scopes).Any()).ToArray();

        return res;
    }

    public async Task<IEnumerable<ApiScope>> FindApiScopesByNameAsync(IEnumerable<string> scopeNames)
    {
        if (scopeNames == null)
            throw new ArgumentNullException(nameof(scopeNames));
        
        await Task.CompletedTask;

        var scopeNamesArray = scopeNames.ToArray();

        var res = IdentityServerStaticConfig.TestApiScopes().Where(x => scopeNamesArray.Contains(x.Name)).ToArray();

        return res;
    }

    public async Task<IEnumerable<IdentityResource>> FindIdentityResourcesByScopeNameAsync(IEnumerable<string> scopeNames)
    {
        if (scopeNames == null)
            throw new ArgumentNullException(nameof(scopeNames));
        
        await Task.CompletedTask;

        var scopeNamesArray = scopeNames.ToArray();

        var res = IdentityServerStaticConfig.TestIdentityResource().Where(x => scopeNamesArray.Contains(x.Name)).ToArray();

        return res;
    }

    public async Task<Resources> GetAllResourcesAsync()
    {
        await Task.CompletedTask;

        var res = new Resources()
        {
            ApiScopes = IdentityServerStaticConfig.TestApiScopes(),
            ApiResources = IdentityServerStaticConfig.TestApiResource(),
            IdentityResources = IdentityServerStaticConfig.TestIdentityResource(),
            OfflineAccess = true
        };

        return res;
    }
}