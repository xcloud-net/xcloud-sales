using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace XCloud.Core.ExceptionHandler;

public interface IExceptionHandler
{
    int Order { get; }
    Task<ErrorInfo> ConvertToErrorInfoOrNull(HttpContext httpContext, Exception e);
}