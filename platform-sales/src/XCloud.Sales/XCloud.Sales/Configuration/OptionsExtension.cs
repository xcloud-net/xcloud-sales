using Volo.Abp.Modularity;
using XCloud.Platform.Core.Database;
using XCloud.Platform.Core.Job;
using XCloud.Sales.Core.Settings;
using XCloud.Sales.Data.Database;

namespace XCloud.Sales.Configuration;

public static class OptionsExtension
{
    public static void ConfigSalesOptions(this ServiceConfigurationContext context)
    {
        var configuration = context.Services.GetConfiguration();

        context.Services.Configure<SalesJobOption>(option => { option.AutoStartJob = true; });
        
        //
        context.Services.Configure<PlatformJobOption>(option =>
        {
            //close platform jobs manually
            option.AutoStartJob = false;
        });
        context.Services.Configure<PlatformDatabaseOption>(option => { option.AutoCreateDatabase = false; });
        context.Services.Configure<SalesDatabaseOption>(option => { option.AutoCreateDatabase = false; });
    }
}