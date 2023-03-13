using System;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Volo.Abp;
using XCloud.Core;
using XCloud.Core.Configuration;
using XCloud.Core.Extension;
using XCloud.Core.Json;

namespace XCloud.AspNetMvc.ModelBinder.JsonModel;

/// <summary>
/// https://docs.microsoft.com/zh-cn/aspnet/core/mvc/advanced/custom-model-binding?view=aspnetcore-3.1#custom-model-binder-sample
/// </summary>
public class JsonModelBinder : IModelBinder
{
    public JsonModelBinder()
    {
    }

    async Task<string> ReadPostBodyString(ModelBindingContext bindingContext, Encoding encoding)
    {
        using var ms = new System.IO.MemoryStream();

        await bindingContext.HttpContext.Request.Body.CopyToAsync(ms);

        var json = encoding.GetString(ms.ToArray()); //IOHelper.StreamToString(ms);

        json.Should().NotBeNullOrEmpty("无法获取请求参数");

        return json;
    }

    public async Task BindModelAsync(ModelBindingContext bindingContext)
    {
        var serviceProvider = bindingContext.HttpContext.RequestServices;
        var logger = serviceProvider.GetRequiredService<ILogger<JsonModelBinder>>();
        var jsonDataSerializer = serviceProvider.GetRequiredService<IJsonDataSerializer>();
        var appConfig = serviceProvider.GetRequiredService<AppConfig>();
        try
        {
            (bindingContext.ModelMetadata.BindingSource == BindingSource.Body).Should()
                .BeTrue("json data必须from body");

            var value = await this.ReadPostBodyString(bindingContext, appConfig.Encoding);

#if DEBUG
            logger.LogDebug($"model绑定原始数据：{value}");
#endif

            var model = jsonDataSerializer.DeserializeFromString(value, bindingContext.ModelType);
            if (model == null)
                throw new AbpException("mvc bind model is null");

            bindingContext.Result = ModelBindingResult.Success(model);
        }
        catch (Exception e)
        {
            logger.AddErrorLog(e.Message, e);
            throw new NoParamException("参数无法解析", e);
        }
    }
}