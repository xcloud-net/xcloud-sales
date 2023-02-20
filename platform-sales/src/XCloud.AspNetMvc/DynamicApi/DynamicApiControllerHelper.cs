using System;
using Microsoft.AspNetCore.Mvc;
using Volo.Abp.Application.Services;
using XCloud.Core.Extension;
using XCloud.Core.Http.Dynamic.Definition;

namespace XCloud.AspNetMvc.DynamicApi;

public static class DynamicApiControllerHelper
{
    public static bool IsDynamicApiController(Type typeInfo)
    {
        if (typeInfo.IsNormalPublicClass() &&
            typeInfo.IsAssignableTo_<IApplicationService>() &&
            typeInfo.GetCustomAttributes_<NonControllerAttribute>().IsEmtpy() &&
            ServiceDefinitionHelper.GetServiceContractInterfaceOrNull(typeInfo) != null)
        {
            return true;
        }

        return false;
    }
}