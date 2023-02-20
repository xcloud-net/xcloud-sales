using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Text.Encodings.Web;
using System.Threading.Tasks;

namespace XCloud.Platform.Auth.Authentication;

public class MySecureDataFormat : ISecureDataFormat<AuthenticationTicket>
{
    public string Protect(AuthenticationTicket data)
    {
        throw new NotImplementedException();
    }

    public string Protect(AuthenticationTicket data, string purpose)
    {
        throw new NotImplementedException();
    }

    public AuthenticationTicket Unprotect(string protectedText)
    {
        throw new NotImplementedException();
    }

    public AuthenticationTicket Unprotect(string protectedText, string purpose)
    {
        throw new NotImplementedException();
    }
}

public class MyCookieManager : ICookieManager
{
    /// <summary>
    /// �洢�ؼ����ݵ�key
    /// </summary>
    private readonly string _key;
    public MyCookieManager(string _key)
    {
        this._key = _key ?? throw new ArgumentNullException(nameof(_key));
    }

    public void AppendResponseCookie(HttpContext context, string key, string value, CookieOptions options)
    {
        //
    }

    public void DeleteCookie(HttpContext context, string key, CookieOptions options)
    {
        //
    }

    public string GetRequestCookie(HttpContext context, string key)
    {
        throw new NotImplementedException();
    }
}

public class MyCookieAuthentication : CookieAuthenticationHandler
{
    public MyCookieAuthentication(
        IOptionsMonitor<CookieAuthenticationOptions> options,
        ILoggerFactory logger,
        UrlEncoder encoder,
        ISystemClock clock) : base(options, logger, encoder, clock)
    {
        //
    }

    protected override Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        return base.HandleAuthenticateAsync();
    }
}

public class MyCookieAuthenticationEvents : CookieAuthenticationEvents
{
    //
}