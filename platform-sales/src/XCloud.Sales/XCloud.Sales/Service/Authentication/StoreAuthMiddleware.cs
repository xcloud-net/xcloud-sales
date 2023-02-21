using Microsoft.AspNetCore.Http;

namespace XCloud.Sales.Service.Authentication;

public class StoreAuthMiddleware : IMiddleware, IScopedDependency
{
    public StoreAuthMiddleware() { }

    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        //do nothing
        await next.Invoke(context);
    }
}