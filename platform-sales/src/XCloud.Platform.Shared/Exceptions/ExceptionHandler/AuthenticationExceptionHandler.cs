using Microsoft.AspNetCore.Http;
using System;
using System.Net;
using System.Threading.Tasks;
using Volo.Abp.Authorization;
using Volo.Abp.DependencyInjection;
using XCloud.Core.ExceptionHandler;

namespace XCloud.Platform.Shared.Exceptions.ExceptionHandler;

public class AuthenticationExceptionHandler : IExceptionHandler, ISingletonDependency
{
    public AuthenticationExceptionHandler()
    {
        //
    }

    public int Order => int.MaxValue;

    public async Task<ErrorInfo> ConvertToErrorInfoOrNull(HttpContext httpContext, Exception e)
    {
        if (e is NoLoginException noLoginException)
        {
            return new ErrorInfo()
            {
                Message = "login required",
                Details = noLoginException.Message,
                HttpStatusCode = HttpStatusCode.Unauthorized
            };
        }
        else if (e is NotActiveException notActiveException)
        {
            return new ErrorInfo()
            {
                Message = "your account is disabled",
                Details = notActiveException.Message,
            };
        }
        else if (e is NoPermissionException)
        {
            return new ErrorInfo()
            {
                Message = "permission required",
                HttpStatusCode = HttpStatusCode.Forbidden
            };
        }
        else if (e is AbpAuthorizationException abpAuthorizationException)
        {
            return new ErrorInfo()
            {
                Message = "base auth failed",
                Details = abpAuthorizationException.Message,
                HttpStatusCode = HttpStatusCode.Unauthorized
            };
        }

        await Task.CompletedTask;

        return null;
    }
}