global using System;
global using Microsoft.EntityFrameworkCore;
global using Microsoft.Extensions.DependencyInjection;
global using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Modularity;
using XCloud.Database.EntityFrameworkCore.MySQL;
using Volo.Abp.AutoMapper;
using XCloud.Platform.Data.EntityFrameworkCore.Configuration;

namespace XCloud.Platform.Data.EntityFrameworkCore;

[DependsOn(typeof(PlatformDataModule),
    typeof(MySqlDatabaseModule))]
public class PlatformDataEntityFrameworkCoreModule : AbpModule
{
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        context.ConfigureDataAccess();
        this.Configure<PlatformEfCoreOption>(option => option.AutoCreateDatabase = true);
        this.Configure<AbpAutoMapperOptions>(option => option.AddMaps<PlatformDataEntityFrameworkCoreModule>(false));
    }


    public override void OnPreApplicationInitialization(ApplicationInitializationContext context)
    {
        var app = context.GetApplicationBuilder();

        Task.Run(async () => { await app.TryCreatePlatformDatabase(); }).Wait();
    }
}