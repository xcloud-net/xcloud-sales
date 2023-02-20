using Newtonsoft.Json;

using Volo.Abp.DependencyInjection;
using XCloud.Core.DependencyInjection.ServiceWrapper;

namespace XCloud.Core.DataSerializer.NewtonsoftJson;

public interface INewtonsoftJsonOptionAccessor
{
    JsonSerializerSettings SerializerSettings { get; }
}

public class DefaultNewtonsoftJsonOptionAccessor : INewtonsoftJsonOptionAccessor, ITransientDependency
{
    public DefaultNewtonsoftJsonOptionAccessor(ServiceWrapper<JsonSerializerSettings> serviceWrapper)
    {
        this.SerializerSettings = serviceWrapper.Value;
    }

    public JsonSerializerSettings SerializerSettings { get; }
}