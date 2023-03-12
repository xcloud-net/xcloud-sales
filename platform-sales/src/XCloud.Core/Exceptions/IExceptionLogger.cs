using System.Text;
using Microsoft.AspNetCore.Http;

namespace XCloud.Core.Exceptions;

public interface IExceptionLogger<T>
{
    void LogError(System.Exception e, string message = null);
}

public class DefaultExceptionLogger<T> : IExceptionLogger<T>//, IScopedDependency
{
    private readonly IServiceProvider serviceProvider;
    private readonly ILogger<T> logger;

    public DefaultExceptionLogger(IServiceProvider serviceProvider, ILogger<T> logger)
    {
        this.serviceProvider = serviceProvider;
        this.logger = logger;
    }

    public void LogError(System.Exception e, string message)
    {
        using var s = this.serviceProvider.CreateScope();

        var sb = new StringBuilder();
        if (!string.IsNullOrWhiteSpace(message))
        {
            sb.AppendLine(message);
        }
        sb.AppendLine(e.Message);

        var httpContextAccessor = s.ServiceProvider.GetService<IHttpContextAccessor>();
        if (httpContextAccessor != null && httpContextAccessor.HttpContext != null)
        {
            sb.AppendLine(httpContextAccessor.HttpContext.Request.Path);
            sb.AppendLine(httpContextAccessor.HttpContext.Request.Method);
        }

        var contributors = s.ServiceProvider.GetServices<IExceptionDetailContributor>().ToArray();
        foreach (var m in contributors)
        {
            m.Contibute(e, ref sb);
        }

        var msg = sb.ToString();
        this.logger.LogError(exception: e, message: msg);
    }
}