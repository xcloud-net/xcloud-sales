using Volo.Abp.EntityFrameworkCore;
using Volo.Abp.Modularity;
using XCloud.Core;

namespace XCloud.Database.EntityFrameworkCore;

[DependsOn(
    typeof(CoreModule),
    typeof(AbpEntityFrameworkCoreModule)
)]
public class EfCoreModule : AbpModule
{
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        //
    }
}