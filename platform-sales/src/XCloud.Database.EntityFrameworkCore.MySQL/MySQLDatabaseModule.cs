using Volo.Abp.EntityFrameworkCore.MySQL;
using Volo.Abp.Guids;
using Volo.Abp.Modularity;

namespace XCloud.Database.EntityFrameworkCore.MySQL;

[DependsOn(
    typeof(EfCoreModule),
    typeof(AbpEntityFrameworkCoreMySQLModule)
)]
public class MySqlDatabaseModule : AbpModule
{
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        this.Configure<AbpSequentialGuidGeneratorOptions>(option =>
        {
            //适配mysql
            option.DefaultSequentialGuidType = SequentialGuidType.SequentialAsString;
        });
    }
}