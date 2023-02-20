using System.IO;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Volo.Abp;
using XCloud.Core.Application.WorkContext;
using XCloud.Core.Http.Dynamic.Definition;
using XCloud.Core.Http.Dynamic.Interceptor;

namespace XCloud.Core.Http.Dynamic.Proxy;

public static class ApiCallHelper
{
    static bool FriendErrorMessage(HttpResponseMessage response)
    {
        if (response.Headers.TryGetValues("_friend_error_message_", out var values) && values.Any())
        {
            return true;
        }
        return false;
    }

    static async Task TryHandleFriendErrorMessage(HttpResponseMessage response, byte[] bs)
    {
        if (!FriendErrorMessage(response))
        {
            return;
        }

        await Task.CompletedTask;
    }

    static async Task<ReturnType> TryHandleResponseData<ReturnType>(IWorkContext context, HttpResponseMessage response, byte[] bs)
    {
        var responseString = context.AppConfig.Encoding.GetString(bs);

        if (responseString is ReturnType t)
        {
            return t;
        }

        try
        {
            var responseEntity = context.JsonSerializer.DeserializeFromString<ReturnType>(responseString);

            return responseEntity;
        }
        catch
        {
            await Task.CompletedTask;
            throw new ApiResponseSerializerException(responseString, typeof(ReturnType));
        }
    }

    static async Task<byte[]> ReadBytes(IWorkContext context, HttpResponseMessage response)
    {
        var maxSize = 1024 * 1024 * 2;

        if (response.Content.Headers.ContentLength != null)
        {
            if (response.Content.Headers.ContentLength.Value > maxSize)
            {
                throw new UserFriendlyException("返回过大");
            }
        }

        using var stream = await response.Content.ReadAsStreamAsync();
        using var ms = new MemoryStream();

        var buffer = new byte[1024];
        var streamSize = 0;

        while (true)
        {
            var flag = await stream.ReadAsync(buffer, 0, buffer.Length);
            if (flag <= 0)
            {
                break;
            }
            streamSize += flag;
            if (streamSize > maxSize)
            {
                throw new UserFriendlyException("读取流过大");
            }

            await ms.WriteAsync(buffer, 0, flag);
        }

        var bs = ms.ToArray();

        return bs;
    }

    static IEnumerable<InterceptorWrapper> ResolveInterceptor<ClientType>(IWorkContext context)
    {
        //所有拦截器
        var all_interceptors = context.ServiceProvider.ResolveAllRequestInterceptor();
        //客户端拦截器
        var client_interceptors = all_interceptors.Where(x => x.IsInterceptorFor<ClientType>()).ToArray();
        //全局拦截器
        var global_interceptors = all_interceptors.Where(x => x.IsGlobalInterceptor()).ToArray();

        var requestMessageInterceptors = global_interceptors.Concat(client_interceptors).OrderBy(x => x.Order).ToArray();

        return requestMessageInterceptors;
    }

    public static async Task<ReturnType> MakeApiRequest<ClientType, ReturnType>(IWorkContext context, ApiCallDescriptor descriptor)
    {
        //所有拦截器
        var requestMessageInterceptors = ResolveInterceptor<ClientType>(context);

        var httpMessageBuilder = context.ServiceProvider.GetRequiredService<HttpMessageBuilder>();
        using var message = httpMessageBuilder.BuildMessage(descriptor);

        foreach (var handler in requestMessageInterceptors)
        {
            handler.RequestInterceptor.InterceptBeforeRequest(message);
        }

        var client = context.ServiceProvider.GetRequiredService<IHttpClientFactory>().CreateClient(typeof(ClientType).FullName);

        using var response = await client.SendAsync(message, cancellationToken: descriptor.CancellationToken);

        foreach (var handler in requestMessageInterceptors)
        {
            handler.RequestInterceptor.InterceptAfterRequest(response);
        }
        //----------------------

        var bs = await ReadBytes(context, response);

        await TryHandleFriendErrorMessage(response, bs);

        var data = await TryHandleResponseData<ReturnType>(context, response, bs);

        return data;
    }

    public static void ParseParameters(IDictionary<ParameterDefinition, object> args,
        out IDictionary<ParameterDefinition, object> param_args,
        out CancellationToken? token)
    {
        param_args = new Dictionary<ParameterDefinition, object>();
        token = null;

        var token_list = new List<CancellationToken>();

        foreach (var m in args)
        {
            var t = m.Value?.GetType();

            (t == typeof(CancellationToken?)).Should().BeFalse("cancellation token should not be nullable");

            if (m.Value != null && m.Value is CancellationToken tk)
            {
                token_list.Add(tk);
            }
            else
            {
                param_args[m.Key] = m.Value;
            }
        }

        if (token_list.Any())
        {
            (token_list.Count > 1).Should().BeFalse("cancellation token");
            token = token_list.FirstOrDefault();
        }
    }
}