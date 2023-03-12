using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

using SkyApm.AspNetCore.Diagnostics;
using SkyApm.Config;
using SkyApm.Tracing;
using SkyApm.Tracing.Segments;

using System;
using System.Linq;
using System.Threading.Tasks;

using Volo.Abp.DynamicProxy;
using XCloud.Core.DependencyInjection;
using XCloud.Core.Extension;

namespace XCloud.Logging.Apm.SkywalkingApm;

public class SkywalkingApmInterceptor : IAbpInterceptor, IAutoRegistered
{
    private readonly IServiceProvider serviceProvider;
    private readonly ILogger logger;
    public SkywalkingApmInterceptor(IServiceProvider serviceProvider)
    {
        this.serviceProvider = serviceProvider;
        this.logger = serviceProvider.GetRequiredService<ILogger<SkywalkingApmInterceptor>>();
    }

    bool ApmEnabledFor(IAbpMethodInvocation invocation)
    {
        var config = invocation.Method.GetCustomAttributes_<ApmAttribute>().FirstOrDefault();
        if (config != null)
        {
            return !config.Disabled;
        }

        return invocation.Method.IsPublic;
        //if (invocation.Method.ReturnType.IsTask() || invocation.Method.ReturnType.IsValueTask())
        //{
        //    return true;
        //}

        //return false;
    }

    SegmentContext CreateEntrySpan(ITracingContext tracer, string full_name)
    {
        var httpcontext = this.serviceProvider.GetService<IHttpContextAccessor>()?.HttpContext;
        if (httpcontext == null)
        {
            throw new ArgumentNullException(nameof(CreateEntrySpan));
        }

        var carrier = new HttpRequestCarrierHeaderCollection(httpcontext.Request);

        var segment = tracer.CreateEntrySegmentContext(full_name, carrier);
        return segment;
    }

    SegmentContext CreateLocalSpan(ITracingContext tracer, string full_name)
    {
        var segment = tracer.CreateLocalSegmentContext(full_name);
        return segment;
    }

    public async Task InterceptAsync(IAbpMethodInvocation invocation)
    {
        var tracer = this.serviceProvider.GetService<ITracingContext>();

        if (tracer != null && this.ApmEnabledFor(invocation))
        {
            var m = invocation.Method;

            var segment = this.CreateLocalSpan(tracer, $"{m.Name}@{m.DeclaringType.Name}");

            try
            {
                segment.Span.AddTag("FunctionName", m.Name);
                var params_data = string.Join(",", m.GetParameters().Select(x => $"{x.ParameterType.Name} {x.Name}"));
                segment.Span.AddTag("FunctionParameters", params_data);

                await invocation.ProceedAsync();

                //log return data
                //todo
            }
            catch (Exception e)
            {
                segment.Span.ErrorOccurred(e, new TracingConfig() { ExceptionMaxDepth = 3 });
                throw;
            }
            finally
            {
                tracer.Release(segment);
            }
        }
        else
        {
            //如果没有开启apm
            await invocation.ProceedAsync();
        }
    }
}