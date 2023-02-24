using Volo.Abp.Modularity;
using XCloud.Platform.Api;
using XCloud.Platform.Core.Database;
using XCloud.Platform.Core.Job;

namespace XCloud.Platform.Test;

[DependsOn(typeof(PlatformApiModule))]
public class PlatformTestModule : AbpModule
{
    public override void PostConfigureServices(ServiceConfigurationContext context)
    {
        this.Configure<PlatformJobOption>(option => option.AutoStartJob = false);
        this.Configure<PlatformDatabaseOption>(option => option.AutoCreateDatabase = false);
    }
}