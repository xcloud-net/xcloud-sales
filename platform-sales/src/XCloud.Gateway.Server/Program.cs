using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.IO;
using Microsoft.AspNetCore.Builder;
using XCloud.Logging;
using Microsoft.Extensions.DependencyInjection;
using XCloud.AspNetMvc.Configuration;
using XCloud.Core.DependencyInjection;

namespace XCloud.Gateway.Server;

public class Program
{
    public static void Main(string[] args)
    {
        var factory = LoggerFactory.Create(builder => builder.AddStartupLoggerProvider());
        var logger = factory.CreateLogger<Program>();
        try
        {
            var builder = WebApplication.CreateBuilder();

            builder.Host.UseAutofac();
            
            builder.TrySettleUpConfig(configBuilder =>
            {
                var path = Path.Combine(builder.Environment.ContentRootPath, "yarp.json");
                configBuilder.AddJsonFile(path, optional: false);
            });

            builder.Services.AddApplication<GatewayServerModule>();

            var app = builder.Build();

            app.InitializeApplication();
            app.Lifetime.ApplicationStopping.Register(IocContext.Instance.Dispose);

            app.Run();
        }
        catch (Exception e)
        {
            logger.LogError(exception: e, message: e.Message);
            throw;
        }
        finally
        {
            factory.Dispose();
        }
    }
}