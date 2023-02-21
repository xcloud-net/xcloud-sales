using System.IO;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using Steeltoe.Extensions.Configuration.Placeholder;
using XCloud.Database.EntityFrameworkCore.MySQL;
using XCloud.Sales.Core;

namespace XCloud.Sales.Data.Database;

public class ShopMigrationDbContextFactory : IDesignTimeDbContextFactory<ShopDbContext>
{
    public ShopDbContext CreateDbContext(string[] args)
    {
        var optionBuilder = new DbContextOptionsBuilder<ShopDbContext>();
        //optionBuilder.UseSqlite("./xx.db");
        optionBuilder.UseMySqlProvider(this.BuildConfiguration(), SalesDbConnectionNames.SalesCloud);

        return new ShopDbContext(optionBuilder.Options);
    }

    private IConfigurationRoot BuildConfiguration()
    {
        var configBuilder = new ConfigurationBuilder();

        configBuilder.AddJsonFile(
            Path.Combine(Directory.GetCurrentDirectory(), "../XCloud.Sales.Mall.Api/appsettings.json"),
            optional: false);
        configBuilder.AddPlaceholderResolver();

        return configBuilder.Build();
    }
}