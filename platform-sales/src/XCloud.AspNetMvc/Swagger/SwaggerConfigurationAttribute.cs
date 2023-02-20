using System;

namespace XCloud.AspNetMvc.Swagger;

public enum SwaggerMode : int
{
    Api = 1,
    Gateway = 2
}

[AttributeUsage(AttributeTargets.Class)]
public class SwaggerConfigurationAttribute : Attribute
{
    public string ServiceName { get; }
    public string Title { get; }
    public string Description { get; }

    public SwaggerMode SwaggerMode { get; } = SwaggerMode.Api;

    public SwaggerConfigurationAttribute(string service_name, string title,
        string description = null,
        SwaggerMode mode = SwaggerMode.Api)
    {
        this.ServiceName = service_name;
        this.Title = title;
        this.Description = description;
        this.SwaggerMode = mode;
    }
}