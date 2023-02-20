using System.Net.Http;

namespace XCloud.Core.Http.Dynamic.Interceptor;

public interface IRequestInterceptor
{
    HttpRequestMessage InterceptBeforeRequest(HttpRequestMessage httpRequestMessage);
    HttpResponseMessage InterceptAfterRequest(HttpResponseMessage httpResponseMessage);
}