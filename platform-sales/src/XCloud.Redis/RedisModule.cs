using Volo.Abp.Caching;
using Volo.Abp.Modularity;
using XCloud.Core;

namespace XCloud.Redis;

[DependsOn(
    typeof(CoreModule),
    typeof(AbpCachingModule)
    //https://docs.abp.io/en/abp/latest/Caching
    //AbpCachingModule use idistributedCache as default provider
    //AbpCachingStackExchangeRedisModule provides new cache implement
)]
public class RedisModule : AbpModule
{
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        context.AddRedisAll();
    }
}