using Volo.Abp.AutoMapper;
using Volo.Abp.Modularity;
using XCloud.Database.EntityFrameworkCore;
using XCloud.Platform.Core;

namespace XCloud.Platform.Data;

[DependsOn(
    typeof(PlatformCoreModule),
    typeof(EfCoreModule))]
public class PlatformDataModule : AbpModule
{
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        this.Configure<AbpAutoMapperOptions>(option => option.AddMaps<PlatformDataModule>(false));
    }
}