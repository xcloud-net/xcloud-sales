using IdentityServer4.EntityFramework.DbContexts;
using IdentityServer4.EntityFramework.Options;
using Microsoft.EntityFrameworkCore;

namespace XCloud.Platform.Auth.IdentityServer.Database.EntityFrameworkCore;

/// <summary>
/// ids授权数据库
/// </summary>
public class IdentityOperationDbContext : PersistedGrantDbContext<IdentityOperationDbContext>
{
    public IdentityOperationDbContext(
        DbContextOptions<IdentityOperationDbContext> option,
        OperationalStoreOptions o) : base(option, o)
    {
        //
    }
}