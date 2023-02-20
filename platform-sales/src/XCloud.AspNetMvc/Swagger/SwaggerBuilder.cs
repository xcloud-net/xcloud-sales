using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using FluentAssertions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using Swashbuckle.AspNetCore.SwaggerUI;
using XCloud.AspNetMvc.Builder;
using XCloud.Core.Builder;
using XCloud.Core.DependencyInjection.Extension;
using XCloud.Core.Extension;
using XCloud.Core.Helper;

namespace XCloud.AspNetMvc.Swagger;

public static class SwaggerBuilder
{
    static ApiDescription ResolveConflictingActions(IEnumerable<ApiDescription> arr)
    {
        var action = arr.FirstOrDefault();

        if (action.ActionDescriptor != null)
            action.ActionDescriptor.DisplayName = $"{action.ActionDescriptor.DisplayName}-{nameof(ResolveConflictingActions)}";

        return action;
    }

    public static IXCloudBuilder AddSwagger(this IXCloudBuilder builder, string service_name, string title, string description = null)
    {
        var apiAssembly = builder.EntryAssembly;

        var info = new OpenApiInfo()
        {
            Title = title,
            Description = description ?? string.Empty
        };

        var env = builder.Services.GetHostingEnvironment();
        var path = Path.Combine(env.ContentRootPath, $"{apiAssembly.GetName().Name}.xml");

        builder.Services.AddSwaggerGen(option =>
        {
            option.SwaggerDoc(name: service_name, info: info);
            option.DocInclusionPredicate((docName, description) => true);
            option.CustomSchemaIds(type => type.FullName);
            option.ResolveConflictingActions(ResolveConflictingActions);

            option.AddServer(new OpenApiServer()
            {
                Url = string.Empty,
                Description = "default server"
            });
            option.CustomOperationIds(apiDesc =>
            {
                var controllerAction = apiDesc.ActionDescriptor as ControllerActionDescriptor;
                controllerAction.Should().NotBeNull(nameof(ControllerActionDescriptor));
                return controllerAction.ControllerName + "-" + controllerAction.ActionName;
            });

            if (File.Exists(path))
            {
                option.IncludeXmlComments(path);
            }

            option.AddSwaggerSecurityRequirement(builder);
        });
        //https://github.com/domaindrivendev/Swashbuckle.AspNetCore#systemtextjson-stj-vs-newtonsoft
        //builder.Services.AddSwaggerGenNewtonsoftSupport();

        return builder;
    }

    /// <summary>
    /// 默认开启，可以通过配置关闭
    /// </summary>
    /// <param name="_config"></param>
    /// <returns></returns>
    public static bool SwaggerEnabled(this IConfiguration _config)
    {
        var res = (_config["swagger"] ?? "true").ToBool();
        return res;
    }

    public static SwaggerGenOptions AddSwaggerSecurityRequirement(this SwaggerGenOptions option, IXCloudBuilder builder)
    {
        var securityRequirement = new OpenApiSecurityRequirement();

        var bearer_auth = new OpenApiSecurityScheme
        {
            In = ParameterLocation.Header,
            Description = "请输入OAuth接口返回的Token，前置Bearer。示例：Bearer {Token}",
            Name = "Authorization",
            Type = SecuritySchemeType.ApiKey
        };
        option.AddSecurityDefinition("Bearer", bearer_auth);
        securityRequirement.Add(new OpenApiSecurityScheme()
        {
            Reference = new OpenApiReference()
            {
                Type = ReferenceType.SecurityScheme,
                Id = "Bearer"
            }
        }, Array.Empty<string>());

        option.AddSecurityRequirement(securityRequirement);

        return option;
    }

    public static IXCloudBuilder TryAddSwagger(this IXCloudBuilder builder)
    {
        builder.Services.ExistService_(typeof(SwaggerConfigurationAttribute)).Should().BeFalse("你无需调用此方法");

        var config = builder.EntryModuleType.GetCustomAttribute<SwaggerConfigurationAttribute>();

        if (config != null)
        {
            builder.Services.AddTransient(x => config);

            AddSwagger(builder, config.ServiceName, config.Title, config.Description);
        }

        return builder;
    }

    /// <summary>
    /// 如果配置开启，并且依赖注入了相关依赖就开启swagger
    /// </summary>
    /// <param name="builder"></param>
    /// <returns></returns>
    public static MvcPipelineBuilder TryEnableSwagger(this MvcPipelineBuilder builder)
    {
        using var s = builder.App.ApplicationServices.CreateScope();
        var logger = s.ServiceProvider.ResolveLogger<MvcPipelineBuilder>();

        if (!builder.Configuration.SwaggerEnabled())
        {
            logger.LogInformation("swagger没有开启，如需开启请修改配置");
            return builder;
        }

        var swaggerConfig = s.ServiceProvider.GetService<SwaggerConfigurationAttribute>();

        if (swaggerConfig == null)
        {
            logger.LogInformation($"请使用{nameof(SwaggerConfigurationAttribute)}配置swagger");
            return builder;
        }

        if (swaggerConfig.SwaggerMode == SwaggerMode.Api)
        {
            //api
            builder.App.UseSwaggerApiDefinitionJson();
            builder.App.UseSwaggerWithUI(api_services: new[] { swaggerConfig.ServiceName });
        }
        else if (swaggerConfig.SwaggerMode == SwaggerMode.Gateway)
        {
            //gateway
            var microServices = (builder.Configuration["micro_services"] ?? string.Empty).Split(',').ToArray();
            microServices.HasDuplicateItems().Should().BeFalse("微服务配置错误");

            //暴露json文件
            builder.App.UseSwaggerDefaultDefinitionJson();
            builder.App.UseSwaggerWithUI(
                api_services: microServices,
                default_services: new[] { swaggerConfig.ServiceName },
                optionBuilder: option =>
                {
                    option.OAuthClientId("wx-imp");
                    option.OAuthClientSecret("123");
                    //option.OAuth2RedirectUrl("http://localhost:8888/swagger/token-callback");
                });
        }
        else
        {
            throw new NotSupportedException("不支持的swagger类型");
        }

        return builder;
    }

    /// <summary>
    /// 暴露接口定义json
    /// </summary>
    /// <param name="app"></param>
    /// <returns></returns>
    public static IApplicationBuilder UseSwaggerDefaultDefinitionJson(this IApplicationBuilder app)
    {
        //在网关层不要用这种route模板暴露json，否则会被swagger中间件拦截json请求。
        //这时应该另外定义模板，然后把api开头的url交给网关转发到下层服务
        app.UseSwagger(option => option.RouteTemplate = __default_template__("{documentName}"));
        return app;
    }

    /// <summary>
    /// 暴露接口定义json
    /// </summary>
    /// <param name="app"></param>
    /// <returns></returns>
    public static IApplicationBuilder UseSwaggerApiDefinitionJson(this IApplicationBuilder app)
    {
        app.UseSwagger(option => option.RouteTemplate = __api_template__("{documentName}"));
        return app;
    }

    /// <summary>
    /// swagger/{documentName}/swagger.json
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    static string __default_template__(string name) => $"/swagger/{name}/swagger.json";
    static string __api_template__(string name) => $"/api/{name}/swagger/swagger.json";

    public static IApplicationBuilder UseSwaggerWithUI(this IApplicationBuilder app,
        string[] api_services, string[] default_services = null,
        Dictionary<string, string> endpoints = null,
        Action<SwaggerUIOptions> optionBuilder = null)
    {
        api_services.Should().NotBeNull();
        endpoints ??= new Dictionary<string, string>();
        //default template
        if (ValidateHelper.IsNotEmptyCollection(default_services))
        {
            foreach (var name in default_services)
            {
                endpoints[name] = __default_template__(name);
            }
        }

        //gateway template
        foreach (var name in api_services)
        {
            endpoints[name] = __api_template__(name);
        }

        //swagger-ui
        app.UseSwaggerUI(option =>
        {
            //添加json节点
            foreach (var kv in endpoints)
            {
                option.SwaggerEndpoint(url: kv.Value, name: kv.Key);
            }

            optionBuilder?.Invoke(option);
        });
        return app;
    }
}