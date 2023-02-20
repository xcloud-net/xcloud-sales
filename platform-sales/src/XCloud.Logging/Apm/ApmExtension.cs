using System.Linq;
using Microsoft.Extensions.Configuration;
using Volo.Abp.Application.Services;
using Volo.Abp.DependencyInjection;
using XCloud.Core.Attributes;
using XCloud.Core.Extension;

namespace XCloud.Logging.Apm;

public static class ApmExtension
{
    public static bool IsApmEnabled(this IConfiguration config) => config["app:config:apm"]?.ToLower() == "true";

    internal static bool ShouldMetric(this IOnServiceRegistredContext context)
    {
        var config = context.ImplementationType.GetCustomAttributes_<ApmAttribute>().FirstOrDefault();
        if (config != null)
        {
            return !config.Disabled;
        }

        var metric = context.ImplementationType.IsAssignableTo_<IApplicationService>();
        metric &= (context.ImplementationType.IsPublic && !context.ImplementationType.IsGenericType);

        return metric;
    }

}