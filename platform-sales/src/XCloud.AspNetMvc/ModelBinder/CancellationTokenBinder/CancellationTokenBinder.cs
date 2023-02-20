using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace XCloud.AspNetMvc.ModelBinder.CancellationTokenBinder;

public class CancellationTokenBinder : IModelBinder
{
    public async Task BindModelAsync(ModelBindingContext bindingContext)
    {
        await Task.CompletedTask;

        var token = bindingContext.HttpContext.RequestAborted;

        bindingContext.Result = ModelBindingResult.Success(token);
    }
}