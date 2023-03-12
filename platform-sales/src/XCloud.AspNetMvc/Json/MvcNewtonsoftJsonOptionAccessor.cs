using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using XCloud.Core.Json.NewtonsoftJson;

namespace XCloud.AspNetMvc.Json;

public class MvcNewtonsoftJsonOptionAccessor : INewtonsoftJsonOptionAccessor
{
    public JsonSerializerSettings SerializerSettings { get; }

    public MvcNewtonsoftJsonOptionAccessor(IOptions<MvcNewtonsoftJsonOptions> options)
    {
        this.SerializerSettings = options.Value.SerializerSettings;
    }
}