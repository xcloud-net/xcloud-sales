using System.Linq;
using System.Threading.Tasks;
using IdentityServer4.Models;
using IdentityServer4.Validation;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using XCloud.Core.Extension;

namespace XCloud.Platform.Auth.IdentityServer.Provider;

public class PlatformRedirectUriValidator : IRedirectUriValidator
{
    private readonly IWebHostEnvironment _webHostEnvironment;
    private readonly string[] _allowedDomains;
    public PlatformRedirectUriValidator(IConfiguration configuration, IWebHostEnvironment webHostEnvironment)
    {
        this._webHostEnvironment = webHostEnvironment;

        var domainConfig = configuration["allowed_redirect_url_domains"] ?? string.Empty;
        _allowedDomains = domainConfig.Split(',').WhereNotNull().Distinct().ToArray();
    }

    public async Task<bool> IsPostLogoutRedirectUriValidAsync(string requestedUri, Client client)
    {
        if (_webHostEnvironment.IsDevelopment())
        {
            return true;
        }

        if (string.IsNullOrWhiteSpace(requestedUri))
        {
            return false;
        }

        var urls = client?.PostLogoutRedirectUris ?? new string[] { };
        if (urls.Contains(requestedUri))
        {
            return true;
        }

        if (_allowedDomains.Any())
        {
            if (_allowedDomains.Any(x => requestedUri.Contains(x)))
            {
                return true;
            }
        }

        await Task.CompletedTask;
        return false;
    }

    public async Task<bool> IsRedirectUriValidAsync(string requestedUri, Client client)
    {
        if (_webHostEnvironment.IsDevelopment())
        {
            return true;
        }

        if (string.IsNullOrWhiteSpace(requestedUri))
        {
            return false;
        }

        var urls = client?.RedirectUris ?? new string[] { };
        if (urls.Contains(requestedUri))
        {
            return true;
        }

        if (_allowedDomains.Any())
        {
            if (_allowedDomains.Any(x => requestedUri.Contains(x)))
            {
                return true;
            }
        }

        await Task.CompletedTask;
        return false;
    }
}