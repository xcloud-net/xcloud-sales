using System.IO;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using Steeltoe.Extensions.Configuration.Placeholder;
using XCloud.Database.EntityFrameworkCore.MySQL;
using XCloud.Platform.Shared;

namespace XCloud.Platform.Data.EntityFrameworkCore.Database;

public class PlatformMigrationDbContextFactory : IDesignTimeDbContextFactory<PlatformDbContext>
{
    public PlatformDbContext CreateDbContext(string[] args)
    {
        var collection = new ServiceCollection();
        collection.AddLogging(builder => builder.AddConsole());
        var provider = collection.BuildServiceProvider();

        var optionBuilder = new DbContextOptionsBuilder<PlatformDbContext>();
        //optionBuilder.UseSqlite("./xx.db");
        optionBuilder.UseMySqlProvider(this.BuildConfiguration(), PlatformDbConnectionNames.Platform);

        return new PlatformDbContext(provider, optionBuilder.Options);
    }

    private IConfigurationRoot BuildConfiguration()
    {
        var configBuilder = new ConfigurationBuilder();

        configBuilder.AddJsonFile(
            Path.Combine(Directory.GetCurrentDirectory(), "../XCloud.Platform.Api/appsettings.json"), optional: false);
        configBuilder.AddPlaceholderResolver();

        return configBuilder.Build();
    }
}