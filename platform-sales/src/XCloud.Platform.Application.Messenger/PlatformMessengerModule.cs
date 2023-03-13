global using System;
global using System.Linq;
global using Microsoft.Extensions.DependencyInjection;
global using Microsoft.Extensions.Logging;
global using Volo.Abp;
global using System.Threading.Tasks;
global using System.Threading;
using Volo.Abp.Modularity;
using XCloud.Core;
using XCloud.Job;
using XCloud.Platform.Application.Messenger.Settings;
using XCloud.Redis;

namespace XCloud.Platform.Application.Messenger;

[DependsOn(
    typeof(CoreModule),
    typeof(JobModule),
    typeof(RedisModule))]
public class PlatformMessengerModule : AbpModule
{
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        this.Configure<PlatformMessengerOption>(option => { });
    }
}