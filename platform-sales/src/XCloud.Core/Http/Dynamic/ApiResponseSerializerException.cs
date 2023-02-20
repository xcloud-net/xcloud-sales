using Volo.Abp;

namespace XCloud.Core.Http.Dynamic;

public class ApiResponseSerializerException : AbpException
{
    public string ResponseString { get; }
    public Type ResponseType { get; }

    public ApiResponseSerializerException(string responseString, Type responseType)
    {
        this.ResponseString = responseString;
        this.ResponseType = responseType;
    }
}