using System;
using Microsoft.AspNetCore.Mvc;

namespace XCloud.AspNetMvc.ModelBinder.CancellationTokenBinder;

[AttributeUsage(AttributeTargets.Parameter)]
public class InjectCancellationTokenAttribute : ModelBinderAttribute
{
    public InjectCancellationTokenAttribute() : base(typeof(ModelBinder.CancellationTokenBinder.CancellationTokenBinder))
    {
        this.BindingSource = Microsoft.AspNetCore.Mvc.ModelBinding.BindingSource.Custom;
    }
}