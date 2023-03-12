using System.Text.Json;
using Volo.Abp.DependencyInjection;
using XCloud.Core.DependencyInjection.ServiceWrapper;

namespace XCloud.Core.Json.TextJson;

public interface ITextJsonOptionAccessor
{
    JsonSerializerOptions SerializerSettings { get; }
}

public class DefaultTextJsonOptionAccessor : ITextJsonOptionAccessor, ITransientDependency
{
    public DefaultTextJsonOptionAccessor(ServiceWrapper<JsonSerializerOptions> serviceWrapper)
    {
        this.SerializerSettings = serviceWrapper.Value;
    }

    public JsonSerializerOptions SerializerSettings { get; }
}