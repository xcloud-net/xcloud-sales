using System;
using System.Linq;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using Volo.Abp.DependencyInjection;
using Volo.Abp.DynamicProxy;
using XCloud.Core.Extension;

namespace XCloud.Logging;

public class LogExceptionSilenceAttribute : Attribute
{
    //
}

public class LogExceptionInterceptor : IAbpInterceptor, ITransientDependency
{
    private readonly ILoggerFactory _loggerFactory;

    public LogExceptionInterceptor(ILoggerFactory loggerFactory)
    {
        this._loggerFactory = loggerFactory;
    }

    public async Task InterceptAsync(IAbpMethodInvocation invocation)
    {
        try
        {
            await invocation.ProceedAsync();
        }
        catch (Exception e)
        {
            var flag = invocation.Method.GetCustomAttributes_<LogExceptionSilenceAttribute>().FirstOrDefault();
            if (flag == null)
            {
                throw;
            }
            else
            {
                var logger =
                    this._loggerFactory.CreateLogger(invocation.Method.DeclaringType ??
                                                     typeof(LogExceptionInterceptor));
                logger.LogError(message: e.Message, exception: e);
            }
        }
    }
}