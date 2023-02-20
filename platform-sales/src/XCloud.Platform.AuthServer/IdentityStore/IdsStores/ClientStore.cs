// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.


using System.Linq;
using System.Threading.Tasks;
using IdentityServer4.Models;
using IdentityServer4.Stores;
using Microsoft.Extensions.Logging;
using XCloud.Core.Application.WorkContext;

namespace XCloud.Platform.AuthServer.IdentityStore.IdsStores;

/// <summary>
/// Implementation of IClientStore thats uses EF.
/// </summary>
/// <seealso cref="IdentityServer4.Stores.IClientStore" />
public class ClientStore : IClientStore
{
    private readonly IWorkContext _context;

    public ClientStore(IWorkContext<ClientStore> context)
    {
        this._context = context;
    }

    /// <summary>
    /// Finds a client by id
    /// </summary>
    /// <param name="clientId">The client id</param>
    /// <returns>
    /// The client
    /// </returns>
    public virtual async Task<Client> FindClientByIdAsync(string clientId)
    {
        await Task.CompletedTask;

        var res = IdentityConfig.TestClients().Where(x => x.ClientId == clientId).FirstOrDefault();

#if DEBUG
        var json = this._context.JsonSerializer.SerializeToString(new { res });
        this._context.Logger.LogInformation($"[{nameof(FindClientByIdAsync)}]{clientId}:{json}");
#endif

        return res;
    }
}