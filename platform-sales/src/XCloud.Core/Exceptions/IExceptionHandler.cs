using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace XCloud.Core.Exceptions;

public interface IExceptionHandler
{
    int Order { get; }
    Task<ErrorInfo> ConvertToErrorInfoOrNull(HttpContext httpContext, System.Exception e);
}