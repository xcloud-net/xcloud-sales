using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using XCloud.AspNetMvc.Builder;
using XCloud.Core.DataSerializer;
using XCloud.Core.Development;
using XCloud.Core.Extension;

namespace XCloud.AspNetMvc.Middleware;

public static class MiddlewareExtension
{
    public static IApplicationBuilder UseAliveCheck(this IApplicationBuilder app)
    {
        app.Map("/alive", builder =>
        {
            builder.Run(async context =>
            {
                var headers = context.Request.Headers.ToDict(x => x.Key, x => x.Value);

                var payload = new
                {
                    headers,
                    server_time_utc = DateTime.UtcNow,
                    alived = true
                };

                var json = context.RequestServices.GetRequiredService<IJsonDataSerializer>().SerializeToString(payload);

                context.Response.StatusCode = 200;
                await context.Response.WriteAsync(json);
            });
        });
        return app;
    }

    public static MvcPipelineBuilder UseDevelopmentInformation(this MvcPipelineBuilder builder)
    {
        var app = builder.App;

        app.Map("/dev", option =>
        {
            option.Run(async context =>
            {
                var data = await context.RequestServices.GetDevelopmentInformation();
                var json = context.RequestServices.GetRequiredService<IJsonDataSerializer>().SerializeToString(data);

                context.Response.StatusCode = 200;
                context.Response.ContentType = "application/json";
                await context.Response.WriteAsync(json);
            });
        });

        return builder;
    }
}