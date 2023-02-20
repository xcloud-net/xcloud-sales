namespace XCloud.Core.Http.Dynamic.Interceptor;

public static class InterceptorExtension
{
    public static IServiceCollection AddHttpRequestInterceptor<T>(this IServiceCollection collection, Type clientType = null) where T : class, IRequestInterceptor
    {
        collection.AddTransient<T>();
        collection.AddTransient<InterceptorWrapper>(provider => new InterceptorWrapper<T>(provider.GetRequiredService<T>(), clientType));
        return collection;
    }

    public static IEnumerable<InterceptorWrapper> ResolveAllRequestInterceptor(this IServiceProvider serviceProvider)
    {
        var res = serviceProvider.GetServices<InterceptorWrapper>().ToArray();
        return res;
    }
}