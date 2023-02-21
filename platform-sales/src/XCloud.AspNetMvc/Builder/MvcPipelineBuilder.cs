using FluentAssertions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Volo.Abp;
using XCloud.AspNetMvc.Extension;
using XCloud.Core.DependencyInjection.Extension;

namespace XCloud.AspNetMvc.Builder;

public class MvcPipelineBuilder
{
    public IApplicationBuilder App { get; }
    public IWebHostEnvironment Environment { get; }
    public IConfiguration Configuration { get; }
    public MvcPipelineBuilder(ApplicationInitializationContext context)
    {
        context.Should().NotBeNull();
        this.App = context.GetApplicationBuilder();
        this.Environment = context.GetEnvironment();
        this.Configuration = context.GetConfiguration();
    }
    public MvcPipelineBuilder(IApplicationBuilder app)
    {
        app.Should().NotBeNull();
        this.App = app;
        this.Environment = app.ApplicationServices.ResolveHostingEnvironment();
        this.Configuration = app.ApplicationServices.ResolveConfiguration();
    }
}