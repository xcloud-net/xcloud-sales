using Volo.Abp;

namespace XCloud.Core.Json;

public class SerializeException : AbpException
{
    public SerializeException(string message) : base(message: message) { }
    public SerializeException(string message, System.Exception e) : base(message, e) { }
}