using System;
using Microsoft.AspNetCore.Mvc;

namespace XCloud.AspNetMvc.ModelBinder.JsonModel;

[AttributeUsage(AttributeTargets.Parameter)]
public class JsonDataAttribute : ModelBinderAttribute
{
    public JsonDataAttribute() : base(typeof(JsonModelBinder))
    {
        this.BindingSource = Microsoft.AspNetCore.Mvc.ModelBinding.BindingSource.Body;
    }
}