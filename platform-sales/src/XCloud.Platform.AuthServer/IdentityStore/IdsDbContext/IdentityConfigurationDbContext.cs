using IdentityServer4.EntityFramework.DbContexts;
using IdentityServer4.EntityFramework.Options;
using Microsoft.EntityFrameworkCore;

namespace XCloud.Platform.AuthServer.IdentityStore.IdsDbContext;

/// <summary>
/// ids配置数据库
/// </summary>
public class IdentityConfigurationDbContext : ConfigurationDbContext<IdentityConfigurationDbContext>
{
    public IdentityConfigurationDbContext(
        DbContextOptions<IdentityConfigurationDbContext> option,
        ConfigurationStoreOptions o) : base(option, o)
    {
        //
    }
}