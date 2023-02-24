// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.


using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using IdentityServer4.Models;
using IdentityServer4.Stores;

namespace XCloud.Platform.AuthServer.IdentityServer.PersistentStore.InMemory;

/// <summary>
/// Implementation of IPersistedGrantStore thats uses EF.
/// </summary>
/// <seealso cref="IdentityServer4.Stores.IPersistedGrantStore" />
[Obsolete]
public class PersistedGrantStore : IPersistedGrantStore
{
    public Task<IEnumerable<PersistedGrant>> GetAllAsync(PersistedGrantFilter filter)
    {
        throw new System.NotImplementedException();
    }

    public Task<PersistedGrant> GetAsync(string key)
    {
        throw new System.NotImplementedException();
    }

    public Task RemoveAllAsync(PersistedGrantFilter filter)
    {
        throw new System.NotImplementedException();
    }

    public Task RemoveAsync(string key)
    {
        throw new System.NotImplementedException();
    }

    public Task StoreAsync(PersistedGrant grant)
    {
        throw new System.NotImplementedException();
    }
}