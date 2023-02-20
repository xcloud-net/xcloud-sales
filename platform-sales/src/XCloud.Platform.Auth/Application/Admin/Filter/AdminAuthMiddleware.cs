using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Volo.Abp;
using XCloud.Core.Application.WorkContext;
using XCloud.Core.Extension;

namespace XCloud.Platform.Auth.Application.Admin.Filter;

public class AdminAuthMiddleware : IMiddleware
{
    public AdminAuthMiddleware() 
    {
        //
    }

    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        var provider = context.RequestServices;
        var workContext = provider.GetRequiredService<IWorkContext<AdminAuthMiddleware>>();

        try
        {
            //
        }
        catch (BusinessException e)
        {
#if DEBUG
            workContext.Logger.LogDebug(e.Message);
#endif
        }
        catch (Exception e)
        {
            workContext.Logger.AddErrorLog("在中间件中加载登陆用户抛出异常", e);
        }
        finally
        {
            //不管是否加载成功都放行
            await next.Invoke(context);
        }
    }
}