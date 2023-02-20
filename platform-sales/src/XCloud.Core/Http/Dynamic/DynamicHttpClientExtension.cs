using System.Text.RegularExpressions;
using Castle.DynamicProxy;
using FluentAssertions;
using Volo.Abp.Application.Services;
using XCloud.Core.Builder;
using XCloud.Core.DependencyInjection.ServiceWrapper;
using XCloud.Core.Http.Dynamic.Proxy;

namespace XCloud.Core.Http.Dynamic;

public static class DynamicHttpClientExtension
{
    public static IXCloudBuilder AddDynamicHttpClient<T>(this IXCloudBuilder builder) where T : class, IApplicationService
    {
        builder.Services.RemoveAll<T>();
        builder.Services.AddTransient<T>(provider => __proxy_client__<T>(provider));

        return builder;
    }

    static T __proxy_client__<T>(IServiceProvider provider) where T : class, IApplicationService
    {
        var proxy = provider.GetRequiredService<DynamicHttpClientProxy>();

        var generator = provider.GetRequiredService<ServiceWrapper<ProxyGenerator>>().Value; //new ProxyGenerator();

        var client = generator.CreateInterfaceProxyWithoutTarget<T>(interceptors: proxy);

        //client = generator.CreateInterfaceProxyWithTarget<T>(target: client, new PolicyProxy(provider));

        return client;
    }

    static readonly Regex re = new Regex(@"\{([^\}]+)\}", RegexOptions.Compiled);
    internal static string ReplacePathArgs(this string path, IDictionary<string, object> args)
    {
        path.Should().NotBeNullOrEmpty();
        args.Should().NotBeNull();

        var res = re.Replace(path, x => args[x.Groups[1].Value]?.ToString());
        return res;
    }
}