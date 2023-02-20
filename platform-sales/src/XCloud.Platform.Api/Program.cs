using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using XCloud.AspNetMvc.Configuration;
using XCloud.Core.DependencyInjection;
using XCloud.Logging;

namespace XCloud.Platform.Api;

public class Program
{
    public static void Main(string[] args)
    {
        var factory = LoggerFactory.Create(builder => builder.AddStartupLoggerProvider());
        var logger = factory.CreateLogger<Program>();
        try
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Host.UseAutofac();

            //builder.Configuration.TryAddNacosConfigProvider();
            
            builder.TrySettleUpConfig(configBuilder => { });
            
            builder.Services.AddApplication<PlatformApiModule>();

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