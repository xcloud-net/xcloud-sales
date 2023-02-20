using System.Reflection;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Routing;
using XCloud.Core.Dto;

namespace XCloud.Core.Http.Dynamic.Definition;

public class ActionDefinition
{
    public ServiceDefinition ServiceDefinition { get; }
    public MethodInfo ActionMethod { get; }

    public IRouteTemplateProvider RouteTemplateProvider { get; }
    public IActionHttpMethodProvider ActionHttpMethodProvider { get; }

    public IReadOnlyList<ParameterDefinition> ParameterDefinitions { get; }

    public ActionDefinition(ServiceDefinition serviceContractDefinition, MethodInfo m)
    {
        serviceContractDefinition.Should().NotBeNull();
        m.Should().NotBeNull();

        this.ServiceDefinition = serviceContractDefinition;
        this.ActionMethod = m;
        var attrs = m.GetCustomAttributes();
        this.RouteTemplateProvider = attrs.OfType<IRouteTemplateProvider>().FirstOrDefault();
        this.ActionHttpMethodProvider = attrs.OfType<IActionHttpMethodProvider>().FirstOrDefault();

        var parameters = m.GetParameters();
        this.ParameterDefinitions = parameters.Select(x => new ParameterDefinition(x)).ToArray();
    }

    public string RouteOrEmpty() => this.RouteTemplateProvider?.Template ?? string.Empty;

    public string ActionHttpMethod() => this.ActionHttpMethodProvider?.HttpMethods?.FirstOrDefault();

    static MethodInfo __get_dispose_method__()
    {
        var m = typeof(IDisposable).GetMethods().FirstOrDefault(x => x.Name == nameof(IDisposable.Dispose));
        m.Should().NotBeNull();
        return m;
    }

    static readonly MethodInfo __dispose_method__ = __get_dispose_method__();

    public bool IsDisposeMethod() => this.ActionMethod == __dispose_method__;

    public bool IsAsyncMethod() => this.ActionMethod.ReturnType.IsTask();

    public Type GetTaskRealReturnType() => this.ActionMethod.ReturnType.UnWrapTaskOrNull();

    public IDictionary<ParameterDefinition, object> GetMethodParameters(object[] args)
    {
        args.Should().NotBeNull();

        (args.Length == this.ParameterDefinitions.Count).Should().BeTrue();

        var res = this.ParameterDefinitions.OrderBy(x => x.ParameterInfo.Position)
            .ToDict(x => x, x => args[x.ParameterInfo.Position]);

        return res;
    }

    public bool IsAction()
    {
        if (this.ParameterDefinitions.Any(x => !x.IsValidated()))
        {
            return false;
        }

        var m = this.ActionMethod;
        if (m.GetCustomAttributes_<NonActionAttribute>().Any())
        {
            return false;
        }

        if (this.RouteTemplateProvider == null || this.ActionHttpMethodProvider == null)
        {
            return false;
        }

        if (m.ReturnType == typeof(void) || m.ReturnType == typeof(Task))
        {
            return false;
        }

        var returnType = this.GetTaskRealReturnType();
        if (returnType != null)
        {
            var normalResponse = returnType == typeof(string) || IsResponseEntity(returnType);
            if (!normalResponse)
            {
                return false;
            }
        }

        if (!m.IsPublic || m.IsGenericMethod)
        {
            return false;
        }

        var baseDefinition = m.GetBaseDefinition();
        if (baseDefinition != null)
        {
            if (baseDefinition.DeclaringType == typeof(object))
            {
                return false;
            }
        }

        return true;
    }

    private bool IsResponseEntity(Type t)
    {
        var res = (t.IsGenericType && t.IsGenericType_(typeof(ApiResponse<>)));
        return res;
    }
}