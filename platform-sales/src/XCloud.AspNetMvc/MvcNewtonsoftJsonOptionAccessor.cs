using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using XCloud.Core.DataSerializer.NewtonsoftJson;

namespace XCloud.AspNetMvc;

public class MvcNewtonsoftJsonOptionAccessor : INewtonsoftJsonOptionAccessor
{
    public JsonSerializerSettings SerializerSettings { get; }

    public MvcNewtonsoftJsonOptionAccessor(IOptions<MvcNewtonsoftJsonOptions> options)
    {
        this.SerializerSettings = options.Value.SerializerSettings;
    }
}