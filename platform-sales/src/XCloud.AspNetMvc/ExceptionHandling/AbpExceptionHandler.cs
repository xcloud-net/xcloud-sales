using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

using System;
using System.Threading.Tasks;

using Volo.Abp.AspNetCore.ExceptionHandling;
using XCloud.Core.Exceptions;

namespace XCloud.AspNetMvc.ExceptionHandling;

public class AbpExceptionHandler : IExceptionHandler//, ISingleInstance
{
    private readonly IServiceProvider serviceProvider;

    public AbpExceptionHandler(IServiceProvider serviceProvider)
    {
        this.serviceProvider = serviceProvider;
    }

    public int Order => int.MinValue;

    public async Task<ErrorInfo> ConvertToErrorInfoOrNull(HttpContext httpContext, Exception e)
    {
        using var s = this.serviceProvider.CreateScope();

        var exceptionHandlingOptions = s.ServiceProvider.GetRequiredService<IOptions<AbpExceptionHandlingOptions>>().Value;
        var exceptionToErrorInfoConverter = s.ServiceProvider.GetRequiredService<IExceptionToErrorInfoConverter>();
        //get exception detail from abp system
        var remoteServiceErrorInfo = exceptionToErrorInfoConverter.Convert(e, options =>
        {
            options.SendExceptionsDetailsToClients = exceptionHandlingOptions.SendExceptionsDetailsToClients;
            options.SendStackTraceToClients = exceptionHandlingOptions.SendStackTraceToClients;
        });

        if (remoteServiceErrorInfo == null)
            return null;

        var error = new ErrorInfo(remoteServiceErrorInfo);

        var exceptionStatusCodeFinder = s.ServiceProvider.GetRequiredService<IHttpExceptionStatusCodeFinder>();
        error.HttpStatusCode = exceptionStatusCodeFinder.GetStatusCode(httpContext, e);

        await Task.CompletedTask;

        return error;
    }
}