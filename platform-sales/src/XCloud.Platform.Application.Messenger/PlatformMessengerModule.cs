global using System;
global using System.Linq;
global using Microsoft.Extensions.DependencyInjection;
global using Microsoft.Extensions.Logging;
global using Volo.Abp;
global using System.Threading.Tasks;
global using System.Threading;
using Volo.Abp.AutoMapper;
using Volo.Abp.Modularity;
using XCloud.Job;
using XCloud.Platform.Application.Messenger.Settings;
using XCloud.Platform.Auth;
using XCloud.Redis;

namespace XCloud.Platform.Application.Messenger;

[DependsOn(
    typeof(JobModule),
    typeof(RedisModule),
    typeof(PlatformAuthModule))]
public class PlatformMessengerModule : AbpModule
{
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        this.Configure<AbpAutoMapperOptions>(option => option.AddMaps<PlatformMessengerModule>(validate: false));
        this.Configure<PlatformMessengerOption>(option => { option.Enabled = false; });
    }
}