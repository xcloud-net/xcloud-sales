using Microsoft.Extensions.DependencyInjection;
using System.Linq;
using Volo.Abp.Auditing;
using Volo.Abp.Modularity;
using XCloud.Core;
using XCloud.Core.Extension;
using XCloud.Logging.Apm;
using XCloud.Logging.Apm.SkywalkingApm;

namespace XCloud.Logging;

[DependsOn(
    typeof(CoreModule),
    typeof(AbpAuditingModule)
)]
public class LoggingModule : AbpModule
{
    public override void PreConfigureServices(ServiceConfigurationContext context)
    {
        var config = context.Services.GetConfiguration();
        var apmEnabled = config.IsApmEnabled();

        context.Services.OnRegistred(ctx =>
        {
            if (apmEnabled && ctx.ShouldMetric())
            {
                /*
                 Notice that OnRegistred callback might be called multiple times for the same service class 
                if it exposes more than one service/interface.
                So, it's safe to use Interceptors.TryAdd method instead of Interceptors.Add method. 
                See the documentation of dynamic proxying / interceptors.
                 */

                //context.Interceptors.Add<ElasticApmInterceptor>();
                ctx.Interceptors.TryAdd<SkywalkingApmInterceptor>();
            }

            if (ctx.ImplementationType.GetCustomAttributes_<LogExceptionSilenceAttribute>().Any())
                ctx.Interceptors.TryAdd<LogExceptionInterceptor>();
        });
    }

    public override void PostConfigureServices(ServiceConfigurationContext context)
    {
        context.AddLoggingAll();
        context.ConfigAuditing();
    }
}