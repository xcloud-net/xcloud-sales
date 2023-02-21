global using System.Linq;
using Volo.Abp.Localization;
using Volo.Abp.Modularity;
using XCloud.Core;
using XCloud.Platform.Shared.Configuration;

namespace XCloud.Platform.Shared;

[DependsOn(
    typeof(CoreModule),
    typeof(AbpLocalizationModule)
)]
public class PlatformSharedModule : AbpModule
{
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        context.ConfigPlatformLocalization();
    }
}