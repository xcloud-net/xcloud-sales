using Microsoft.AspNetCore.Http;
using XCloud.AspNetMvc;
using XCloud.AspNetMvc.Extension;

namespace XCloud.Sales.Core;

/// <summary>
/// Represents a common helper
/// </summary>
public interface IWebHelper
{
    /// <summary>
    /// Get context IP address
    /// </summary>
    /// <returns>URL referrer</returns>
    string GetCurrentIpAddress();
}

public class WebHelper : IWebHelper, ITransientDependency
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public WebHelper(
        IHttpContextAccessor httpContextAccessor)
    {
        this._httpContextAccessor = httpContextAccessor;
    }

    /// <summary>
    /// Get IP address from HTTP context
    /// </summary>
    /// <returns>String of IP address</returns>
    public virtual string GetCurrentIpAddress()
    {
        if (_httpContextAccessor.HttpContext==null)
            return string.Empty;

        return this._httpContextAccessor.HttpContext.GetCurrentIpAddress();
    }
}