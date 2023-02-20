using System.Linq;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Cors.Infrastructure;
using Microsoft.Extensions.Configuration;
using XCloud.Core;
using XCloud.Core.Extension;

namespace XCloud.AspNetMvc.Builder;

public static class CorsBuilder
{
    /// <summary>
    /// 允许任意跨域请求
    /// </summary>
    /// <param name="app"></param>
    /// <returns></returns>
    public static IApplicationBuilder UseDevCors(this IApplicationBuilder app)
    {
        void ConfigDev(CorsPolicyBuilder builder)
        {
            builder
                .AllowAnyHeader()
                .AllowAnyMethod()
                //.AllowCredentials()
                .AllowAnyOrigin();
        }

        app.UseCors(ConfigDev);

        return app;
    }

    /// <summary>
    /// 使用生产环境的跨域配置
    /// </summary>
    /// <param name="app"></param>
    /// <param name="config"></param>
    /// <returns></returns>
    public static IApplicationBuilder UseProductionCors(this IApplicationBuilder app, IConfiguration config)
    {
        void ConfigProduction(CorsPolicyBuilder builder)
        {
            var originConfig = config["CorsHosts"] ?? string.Empty;

            var origins = originConfig.Split(',', '\t', '\n').WhereNotEmpty().Distinct().ToArray();

            if (!origins.Any())
            {
                throw new ConfigException("请配置跨域所需的origin hosts");
            }

            builder
                .AllowAnyHeader()
                .AllowAnyMethod()
                .AllowCredentials()
                .WithOrigins(origins: origins);
        }

        app.UseCors(ConfigProduction);

        return app;
    }
}