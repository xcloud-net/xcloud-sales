using Microsoft.Extensions.Configuration;
using Volo.Abp.AspNetCore.Mvc;
using XCloud.Core.Json;

namespace XCloud.AspNetMvc.Controller;

public abstract class XCloudBaseController : AbpController
{
    protected IConfiguration Configuration => 
        this.LazyServiceProvider.LazyGetRequiredService<IConfiguration>();

    protected IJsonDataSerializer JsonDataSerializer =>
        this.LazyServiceProvider.LazyGetRequiredService<IJsonDataSerializer>();
}