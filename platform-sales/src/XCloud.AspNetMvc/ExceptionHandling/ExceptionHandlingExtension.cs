using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Linq;
using Volo.Abp.AspNetCore.Mvc.ExceptionHandling;

namespace XCloud.AspNetMvc.ExceptionHandling;

public static class ExceptionHandlingExtension
{
    static bool DropFilter(Type[] filterTypeToRemove, IFilterMetadata x)
    {
        if (filterTypeToRemove.Contains(x.GetType()))
        {
            return true;
        }

        if (x is ServiceFilterAttribute attr && filterTypeToRemove.Contains(attr?.ServiceType))
        {
            return true;
        }

        return false;
    }

    public static MvcOptions AddExceptionHandler(this MvcOptions option)
    {
        var filterTypeToRemove = new[] { typeof(AbpExceptionFilter), typeof(AbpExceptionPageFilter) };

        var filterToRemove = option.Filters.Where(x => DropFilter(filterTypeToRemove, x)).ToArray();

        foreach (var filter in filterToRemove)
        {
            option.Filters.Remove(filter);
        }

        //abp在AbpAspNetCoreMvcModule就添加了异常处理的过滤器
        //TODO 这里的优先级要考虑下
        option.Filters.AddService<TheMvcExceptionFilter>(order: int.MaxValue);

        return option;
    }
}