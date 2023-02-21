using Volo.Abp.EntityFrameworkCore;
using XCloud.Database.EntityFrameworkCore.Extensions;
using XCloud.Database.EntityFrameworkCore.MySQL.Mapping;
using XCloud.Sales.Core;
using XCloud.Sales.Data.Domain.Common;
using XCloud.Sales.Data.Domain.Promotion;
using XCloud.Sales.Data.Domain.Stores;

namespace XCloud.Sales.Data.Database;

//[ConnectionStringName("XSales")]
public class ShopDbContext : AbpDbContext<ShopDbContext>
{
    public ShopDbContext(DbContextOptions<ShopDbContext> options) : base(options)
    {
        //
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyUtf8Mb4ForAll();
        modelBuilder.ConfigEntityMapperFromAssemblies<ISalesBaseEntity>(new[] { this.GetType().Assembly });
    }

    public DbSet<Pages> Pages { get; set; }
    public DbSet<StorePromotion> Promotions { get; set; }
    public DbSet<Store> Stores { get; set; }
    public DbSet<StoreGoodsMapping> StoreGoodsMappings { get; set; }
    public DbSet<StoreManager> StoreManagers { get; set; }
}