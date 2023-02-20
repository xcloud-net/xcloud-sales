using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Filters;

namespace XCloud.Platform.Auth.Application.Admin.Filter;

public class AdminAuthAttribute : ActionFilterAttribute
{
    /// <summary>
    /// 权限，逗号隔开
    /// </summary>
    public string Permission { get; set; }

    public AdminAuthAttribute(string permission = null)
    {
        this.Permission = permission;
    }

    public override async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        await next.Invoke();
    }
}