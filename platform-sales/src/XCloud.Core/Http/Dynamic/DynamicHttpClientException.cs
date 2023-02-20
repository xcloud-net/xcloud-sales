using Volo.Abp;

namespace XCloud.Core.Http.Dynamic;

public class DynamicHttpClientException : AbpException
{
    public DynamicHttpClientException(string message) : base(message: message) { }
}