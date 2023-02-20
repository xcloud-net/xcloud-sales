using System.Reflection;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Routing;

namespace XCloud.Core.Http.Dynamic.Definition;

public class ServiceDefinition
{
    public Type ServiceInterface { get; }
    public DynamicHttpClientAttribute ClientConfiguration { get; }
    public IRouteTemplateProvider RouteTemplateProvider { get; }

    public IReadOnlyList<ActionDefinition> AllMethodDefinitions { get; }
    public IReadOnlyList<ActionDefinition> ActionDefinitions => this.AllMethodDefinitions.Where(x => x.IsAction()).ToArray();

    public ServiceDefinition(Type serviceContract)
    {
        serviceContract.Should().NotBeNull();
        serviceContract.IsInterface.Should().BeTrue();

        this.ServiceInterface = serviceContract;

        var attrs = serviceContract.GetCustomAttributes();
        this.RouteTemplateProvider = attrs.OfType<IRouteTemplateProvider>().FirstOrDefault();
        this.ClientConfiguration = attrs.OfType<DynamicHttpClientAttribute>().FirstOrDefault();

        this.AllMethodDefinitions = serviceContract.GetMethods().Select(x => new ActionDefinition(this, x)).ToArray();
    }

    public bool IsValidService()
    {
        if (this.ClientConfiguration == null)
        {
            return false;
        }
        return true;
    }

    public string RouteOrEmpty() => this.RouteTemplateProvider?.Template ?? string.Empty;
}