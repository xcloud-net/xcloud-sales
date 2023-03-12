using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

using System;
using System.Text;
using System.Threading.Tasks;

using Volo.Abp.DependencyInjection;
using XCloud.Core.Exceptions;

namespace XCloud.Database.EntityFrameworkCore;

public class EfCoreExceptionHandler : IExceptionHandler, ITransientDependency
{
    public int Order => default;

    string ExceptionDetail(DbUpdateException e)
    {
        var sb = new StringBuilder();

        sb.AppendLine($"message:{e.Message}");
        sb.AppendLine($"detail:{e.StackTrace}");
        sb.AppendLine($"inner_message:{e.InnerException.Message}");
        sb.AppendLine($"inner_detail:{e.InnerException?.Message}");

        return sb.ToString();
    }

    public async Task<ErrorInfo> ConvertToErrorInfoOrNull(HttpContext httpContext, Exception e)
    {
        if (e is DbUpdateException dbUpdateException)
        {
            return new ErrorInfo()
            {
                Message = "db error",
                Details = this.ExceptionDetail(dbUpdateException)
            };
        }

        if (e is DbUpdateConcurrencyException dbUpdateConcurrencyException)
        {
            return new ErrorInfo()
            {
                Message = "db concurrency error",
                Details = this.ExceptionDetail(dbUpdateConcurrencyException)
            };
        }

        await Task.CompletedTask;
        return null;
    }
}