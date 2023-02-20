using FluentAssertions;
using Microsoft.AspNetCore.Mvc;

namespace XCloud.Core.Http.Dynamic;

[AttributeUsage(AttributeTargets.Interface)]
public class DynamicHttpClientAttribute : RouteAttribute
{
    public string ServiceName { get; }

    public DynamicHttpClientAttribute(string serviceName, string template = null) : base(template: template ?? string.Empty)
    {
        serviceName.Should().NotBeNullOrEmpty();
        this.ServiceName = serviceName;
    }
}