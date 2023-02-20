using System.Reflection;
using System.Threading;
using Castle.DynamicProxy;
using FluentAssertions;
using Microsoft.Extensions.Caching.Memory;
using Volo.Abp.Threading;
using XCloud.Core.Application.WorkContext;
using XCloud.Core.DependencyInjection;
using XCloud.Core.Http.Dynamic.Definition;

namespace XCloud.Core.Http.Dynamic.Proxy;

public class DynamicHttpClientProxy : IInterceptor, IDisposable, IAutoRegistered
{
    private readonly IWorkContext context;
    private readonly IMemoryCache memoryCache;

    public DynamicHttpClientProxy(IWorkContext<DynamicHttpClientProxy> context)
    {
        this.context = context;
        this.memoryCache = context.ServiceProvider.GetRequiredService<IMemoryCache>();
    }

    static MethodInfo MakeRealApiCallMethod(ActionDefinition actionDefinition, Type responseType)
    {
        var originApiCallMethod = typeof(ApiCallHelper).GetPublicStaticMethods().FirstOrDefault(x => x.Name == nameof(ApiCallHelper.MakeApiRequest));
        originApiCallMethod.Should().NotBeNull("请注意方法访问级别");

        var realApiCallMethod = originApiCallMethod.MakeGenericMethod(actionDefinition.ServiceDefinition.ServiceInterface, responseType);
        return realApiCallMethod;
    }

    MethodInfo GetCachedRealApiCallMethod(ActionDefinition actionDefinition, Type responseType)
    {
        var method = actionDefinition.ActionMethod;

        //创建泛型方法
        var parameter_flag = string.Join('|', method.GetParameters().Select(x => $"{x.ParameterType.Name}->{x.Name}"));
        var method_flag = $"{method.DeclaringType.FullName}.{method.IsGenericMethod}.{method.Name}.{parameter_flag}".RemoveWhitespace();

        var realApiCallMethod = this.memoryCache.GetOrCreate(method_flag, x =>
        {
            x.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(10);
            return new MethodWrapper(MakeRealApiCallMethod(actionDefinition, responseType));
        })?.Method;

        realApiCallMethod.Should().NotBeNull();

        return realApiCallMethod;
    }

    void ParseMethodParameter(ActionDefinition actionDefinition, IInvocation invocation, out ApiCallDescriptor apiDescriptor)
    {
        var args = actionDefinition.GetMethodParameters(invocation.Arguments);

        ApiCallHelper.ParseParameters(args, out var request_args, out var token);

        if (token == null)
        {
            //如果入参没有取消token，那么尝试从abp的对象中拿（mvc环境下是请求取消）
            var token_provider = this.context.ServiceProvider.GetService<ICancellationTokenProvider>();
            token = token_provider?.Token ?? CancellationToken.None;
        }

        apiDescriptor = new ApiCallDescriptor(actionDefinition, request_args, token.Value);
    }

    /// <summary>
    /// 判断方法是否可以代理
    /// </summary>
    /// <param name="actionDefinition"></param>
    /// <returns></returns>
    static bool IsMethodInterceptorable(ActionDefinition actionDefinition) =>
        actionDefinition.ActionMethod.IsPublic && (!actionDefinition.ActionMethod.IsGenericMethod) &&
        actionDefinition.ServiceDefinition.IsValidService() &&
        actionDefinition.IsAsyncMethod();

    public void Intercept(IInvocation invocation)
    {
        var method = invocation.Method;

        var decalreType = method.DeclaringType;
        decalreType.IsInterface.Should().BeTrue();

        var serviceDefinitionCacheKey = $"{method.DeclaringType.FullName}".RemoveWhitespace();

        var serviceDefinition = this.memoryCache.GetOrCreate(serviceDefinitionCacheKey, x =>
        {
            x.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(10);
            return new ServiceDefinition(decalreType);
        });

        var actionDefinition = serviceDefinition.AllMethodDefinitions.FirstOrDefault(x => x.ActionMethod == method);
        if (actionDefinition == null)
        {
            throw new NotSupportedException("不支持代理");
        }

        if (actionDefinition.IsDisposeMethod())
        {
            this.DisposeClient(invocation);
        }
        else if (IsMethodInterceptorable(actionDefinition))
        {
            if (actionDefinition.ParameterDefinitions.Any(x => !x.IsValidated()))
            {
                throw new DynamicHttpClientException("方法参数配置错误");
            }

            //创建调用代理
            var responseType = actionDefinition.GetTaskRealReturnType();
            if (responseType == null)
            {
                /*
                 如果返回是Task<User>那么构造一个返回Task<User>的调用方法
                 如果返回值Task，那么构造一个Task<string>的调用方法(class Task<string>:Task{})
                 */
                responseType = typeof(string);
            }

            //提取方法参数
            this.ParseMethodParameter(actionDefinition, invocation, out var apiDescriptor);

            //创建泛型方法
            var realApiCallMethod = this.GetCachedRealApiCallMethod(actionDefinition, responseType);

            //执行网络请求
            invocation.ReturnValue = realApiCallMethod.Invoke(
                obj: null,
                parameters: new object[] { this.context, apiDescriptor });
        }
        else
        {
            throw new NotSupportedException("无法生成api调用代理");
        }
    }

    void DisposeClient(IInvocation invocation) { }

    public void Dispose()
    {
#if DEBUG
        this.context.Logger.LogInformation("client disposed");
#endif
    }
}