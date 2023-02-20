using Microsoft.AspNetCore.Mvc.Controllers;
using System.Reflection;

namespace XCloud.AspNetMvc.DynamicApi;

public class DynamicApiControllerFeatureProvider : ControllerFeatureProvider
{
    protected override bool IsController(TypeInfo typeInfo)
    {
        return DynamicApiControllerHelper.IsDynamicApiController(typeInfo);
    }
}