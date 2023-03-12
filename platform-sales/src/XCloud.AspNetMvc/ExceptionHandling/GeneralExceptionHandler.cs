using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Domain.Entities;
using XCloud.Core;
using XCloud.Core.DependencyInjection;
using XCloud.Core.Exceptions;

namespace XCloud.AspNetMvc.ExceptionHandling;

public class GeneralExceptionHandler : IExceptionHandler, ISingleInstance
{
    public GeneralExceptionHandler()
    {
        //
    }

    public int Order => default;

    public async Task<ErrorInfo> ConvertToErrorInfoOrNull(HttpContext httpContext, Exception e)
    {
        if (e is BusinessException businessException)
        {
            return new ErrorInfo()
            {
                Message = businessException.Message,
                Details = businessException.Details,
                Data = businessException.Data,
                FriendlyError = true
            };
        }
        else if (e is EntityNotFoundException entityNotFoundException)
        {
            return new ErrorInfo()
            {
                Message = "data is not exist",
                Details = entityNotFoundException.Message,
            };
        }
        else if (e is NoParamException noParamEx)
        {
            return new ErrorInfo()
            {
                Message = "参数错误",
                Code = "-100",
                Data = new Dictionary<string, object>() { ["template"] = noParamEx.ResponseTemplate },
                HttpStatusCode = HttpStatusCode.BadRequest
            };
        }
        else if (e is NotImplementedException || e is NotSupportedException)
        {
            return new ErrorInfo()
            {
                Message = "该功能未实现或者暂不支持",
                HttpStatusCode = HttpStatusCode.NotImplemented
            };
        }

        await Task.CompletedTask;
        return null;
    }
}