using Microsoft.AspNetCore.Mvc.ModelBinding;
using System;
using System.Threading;
using XCloud.AspNetMvc.ModelBinder.JsonModel;
using XCloud.Core.Helper;

namespace XCloud.AspNetMvc.ModelBinder;

/// <summary>
/// https://docs.microsoft.com/zh-cn/aspnet/core/mvc/advanced/custom-model-binding?view=aspnetcore-3.1#implementing-a-modelbinderprovider
/// </summary>
public class MyModelBinderProvider : IModelBinderProvider
{
    public MyModelBinderProvider() { }

    public IModelBinder GetBinder(ModelBinderProviderContext context)
    {
        if (context.Metadata.ModelType.IsAssignableTo<IDataModelFinder>())
        {
            return new JsonModelBinder();
        }

        if (context.Metadata.ModelType == typeof(CancellationToken) ||
            context.Metadata.ModelType == typeof(CancellationToken?))
        {
            //todo 这里注入abp获取cancellation token的provider
            return new CancellationTokenBinder.CancellationTokenBinder();
        }

        return null;
    }
}