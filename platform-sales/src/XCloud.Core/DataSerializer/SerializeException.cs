using Volo.Abp;

namespace XCloud.Core.DataSerializer;

public class SerializeException : AbpException
{
    public SerializeException(string message) : base(message: message) { }
    public SerializeException(string message, Exception e) : base(message, e) { }
}