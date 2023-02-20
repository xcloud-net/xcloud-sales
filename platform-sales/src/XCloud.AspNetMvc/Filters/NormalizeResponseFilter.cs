using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using XCloud.Core.DependencyInjection;
using XCloud.Core.Extension;

namespace XCloud.AspNetMvc.Filters;

public class NormalizeResponseFilter : IAsyncResultFilter, IAutoRegistered
{
    public async Task OnResultExecutionAsync(ResultExecutingContext context, ResultExecutionDelegate next)
    {
        var logger = context.HttpContext.RequestServices.ResolveLogger<NormalizeResponseFilter>();
        logger.LogInformation($"{nameof(NormalizeResponseFilter)}-{context.HttpContext.Request.Path}");

        if (context.Result is ObjectResult or)
        {
            //
            context.HttpContext.Response.Headers["normalized_response"] = "true";
        }

        await next.Invoke();
    }
}