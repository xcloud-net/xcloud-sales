using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Data;
using XCloud.Core.DependencyInjection;
using XCloud.Core.Dto;
using XCloud.Core.ExceptionHandler;
using XCloud.Logging;

namespace XCloud.AspNetMvc.ExceptionHandling;

public class TheMvcExceptionFilter : IAsyncExceptionFilter, IAutoRegistered
{
    private readonly ILogger _logger;
    private readonly IWebHostEnvironment _environment;
    private readonly IConfiguration _configuration;

    public TheMvcExceptionFilter(ILogger<TheMvcExceptionFilter> logger,
        IWebHostEnvironment environment,
        IConfiguration configuration)
    {
        this._logger = logger;
        this._environment = environment;
        this._configuration = configuration;
    }

    async Task<ErrorInfo> ConvertToErrorInfo(ExceptionContext context, Exception e)
    {
        var handlers = context.HttpContext.RequestServices.GetServices<IExceptionHandler>().ToArray();

        handlers = handlers.OrderByDescending(x => x.Order).ToArray();

        foreach (var h in handlers)
        {
            var response = await h.ConvertToErrorInfoOrNull(context.HttpContext, e);
            if (response != null)
            {
                return response;
            }
        }

        return null;
    }

    bool ShouldHandle(ExceptionContext context, out Exception e)
    {
        e = context.Exception;

        if (e == null || context.ExceptionHandled)
        {
            return false;
        }

        if (context.HttpContext.Response.HasStarted)
        {
            //this.logger.LogError(exception: e, message: e.Message);
            return false;
        }

        return true;
    }

    string BuildExceptionDetail(Exception e)
    {
        var errBuilder = new StringBuilder();
        errBuilder.AppendLine($"-------{e.Message}--------");
        errBuilder.AppendLine(e.StackTrace);
        return errBuilder.ToString();
    }

    public async Task OnExceptionAsync(ExceptionContext context)
    {
        if (!this.ShouldHandle(context, out var e))
            return;

        var error = await ConvertToErrorInfo(context, e);
        if (error == null)
        {
            error = new ErrorInfo()
            {
                FriendlyError = false,
                HttpStatusCode = HttpStatusCode.InternalServerError
            };

            error.Message = "unhandled exception";

            _logger.LogError(message: error.Message, exception: e);
        }

        var response = new ApiResponse<object>() { Error = error };

        if (_environment.IsDevelopment() || _environment.IsStaging() || this._configuration.IsDebug())
        {
            response.SetProperty("dev_info", this.BuildExceptionDetail(e));
        }

        var result = new JsonResult(response);
        if (error.HttpStatusCode != null)
        {
            result.StatusCode = (int)error.HttpStatusCode.Value;
        }

        context.Result = result;
        context.Exception = default;
        context.ExceptionHandled = true;
    }
}