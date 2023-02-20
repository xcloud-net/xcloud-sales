using IdentityServer4.Models;
using IdentityServer4.Validation;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using System.Linq;
using System.Threading.Tasks;
using XCloud.Core.Extension;

namespace XCloud.Platform.AuthServer.Validator;

public class MyRedirectUriValidator : IRedirectUriValidator
{
    private readonly IConfiguration _configuration;
    private readonly IWebHostEnvironment _webHostEnvironment;
    private readonly string[] _allowedDomains;
    public MyRedirectUriValidator(IConfiguration configuration, IWebHostEnvironment webHostEnvironment)
    {
        this._configuration = configuration;
        this._webHostEnvironment = webHostEnvironment;

        var domainConfig = this._configuration["allowed_redirect_url_domains"] ?? string.Empty;
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