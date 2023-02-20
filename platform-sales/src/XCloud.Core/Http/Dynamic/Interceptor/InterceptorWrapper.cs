using FluentAssertions;

namespace XCloud.Core.Http.Dynamic.Interceptor;

public class InterceptorWrapper<T> : InterceptorWrapper where T : class, IRequestInterceptor
{
    public Type InterceptorType { get; }
    public InterceptorWrapper(IRequestInterceptor requestInterceptor, Type type) : base(requestInterceptor, type)
    {
        this.InterceptorType = typeof(T);
    }
}

public class InterceptorWrapper
{
    public int Order { get; }
    public Type TargetType { get; }
    public IRequestInterceptor RequestInterceptor { get; }

    public InterceptorWrapper(IRequestInterceptor requestInterceptor, Type type)
    {
        requestInterceptor.Should().NotBeNull();
        this.RequestInterceptor = requestInterceptor;
        this.TargetType = type;
    }

    public bool IsGlobalInterceptor() => this.TargetType == null;

    public bool IsInterceptorFor<ClientType>() => this.TargetType == typeof(ClientType);
}