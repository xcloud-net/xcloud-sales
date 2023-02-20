using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.DependencyInjection;

namespace XCloud.Logging.Serilog;

public class AspNetCoreEnricherMiddleware : IMiddleware, ITransientDependency
{
    public AspNetCoreEnricherMiddleware() { }

    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        AspNetCoreEnricher.HttpContextAccessor.Value = context.RequestServices.GetRequiredService<IHttpContextAccessor>();
        using (new DisposeAction(() => AspNetCoreEnricher.HttpContextAccessor.Value = null))
        {
            await next.Invoke(context);
        }
    }
}

public static class AspNetCoreEnricherMiddlewareExtension
{
    public static void UseSerilogAspNetCoreEnricher(this IApplicationBuilder app)
    {
        app.UseMiddleware<AspNetCoreEnricherMiddleware>();
    }
}