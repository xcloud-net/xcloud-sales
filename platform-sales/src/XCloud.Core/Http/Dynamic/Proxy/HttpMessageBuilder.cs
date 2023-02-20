using System.Net.Http;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using XCloud.Core.Application.WorkContext;
using XCloud.Core.DependencyInjection;
using XCloud.Core.Http.Content;
using XCloud.Core.Http.Dynamic.Definition;

namespace XCloud.Core.Http.Dynamic.Proxy;

public class HttpMessageBuilder : IAutoRegistered
{
    private readonly IWorkContext context;
    private readonly IDynamicClientServiceDiscovery dynamicClientServiceDiscovery;

    public HttpMessageBuilder(IWorkContext<HttpMessageBuilder> context,
        IDynamicClientServiceDiscovery dynamicClientServiceDiscovery)
    {
        this.context = context;
        this.dynamicClientServiceDiscovery = dynamicClientServiceDiscovery;
    }

    string BuildUrl(string base_url, string path)
    {
        base_url.Should().NotBeNullOrEmpty();
        base_url = base_url.Trim().TrimEnd('/', '\\');

        path ??= string.Empty;
        path = path.Trim().TrimStart('/', '\\');

        var url = $"{base_url}/{path}";
        return url;
    }

    string buildRequestUrl(ApiCallDescriptor descriptor)
    {
        var base_url = this.dynamicClientServiceDiscovery.GetServiceAddress(descriptor.ActionDefinition.ServiceDefinition.ClientConfiguration.ServiceName);
        base_url.Should().NotBeNullOrEmpty();

        var routePrefix = descriptor.ActionDefinition.ServiceDefinition.RouteOrEmpty();
        var path = descriptor.ActionDefinition.RouteOrEmpty();

        var routes = new string[] { routePrefix, path }.Where(x => x?.Length > 0).ToArray();
        routes = routes.SelectMany(x => x.Split('/', '\\')).ToArray();

        var requestPath = string.Join("/", routes);

        var args = descriptor.Args;

        var pathParameters = args.Where(x => x.Key.BindingSource == BindingSource.Path).ToDict(x => x.Key, x => x.Value);
        requestPath = requestPath.ReplacePathArgs(pathParameters.ToDict(x => x.Key.ParameterInfo.Name, x => x.Value));

        var queryParameters = args.Where(x => x.Key.BindingSource == BindingSource.Query).ToDict(x => x.Key, x => x.Value);
        var query_string = queryParameters.ToDict(x => x.Key.ParameterInfo.Name, x => x.Value?.ToString() ?? string.Empty).ToUrlParam();
        requestPath = $"{requestPath}?{query_string}";

        var requestUrl = $"{base_url}/{requestPath}";
        return requestUrl;
    }

    public HttpRequestMessage BuildMessage(ApiCallDescriptor descriptor)
    {
        var args = descriptor.Args;
        var requestUrl = this.buildRequestUrl(descriptor);

        var httpMethod = descriptor.ActionDefinition.ActionHttpMethod();

        if ("GET".Equals(httpMethod, StringComparison.OrdinalIgnoreCase))
        {
            var message = new HttpRequestMessage(HttpMethod.Get, requestUrl);

            return message;
        }
        else if ("POST".Equals(httpMethod, StringComparison.OrdinalIgnoreCase))
        {
            var message = new HttpRequestMessage(HttpMethod.Post, requestUrl);

            var bodyArgs = args.Where(x => x.Key.BindingSource == BindingSource.Body).ToArray();
            if (bodyArgs.Any())
            {
                (message.Content == null).Should().BeTrue();

                (bodyArgs.Length == 1).Should().BeTrue();
                var entity = bodyArgs.FirstOrDefault().Value;

                entity.Should().NotBeNull("post body 参数为空");

                var body = this.context.JsonSerializer.SerializeToString(entity);
                message.Content = new JsonContent(body, this.context.AppConfig.Encoding);
            }

            var formArgs = args.Where(x => x.Key.BindingSource == BindingSource.Form).ToArray();
            if (formArgs.Any())
            {
                (message.Content == null).Should().BeTrue();

                var content = new MultipartFormDataContent();
                foreach (var p in args)
                {
                    content.Add(new StringContent(p.Value?.ToString()), p.Key.ParameterInfo.Name);
                }
                message.Content = content;
            }

            return message;
        }
        else
        {
            throw new NotImplementedException();
        }
    }
}