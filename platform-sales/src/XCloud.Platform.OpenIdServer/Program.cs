using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using XCloud.Logging;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using XCloud.AspNetMvc.Configuration;
using XCloud.Core.DependencyInjection;

namespace XCloud.Platform.OpenIdServer;

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

            builder.TrySettleUpConfig();
            
            builder.Services.AddApplication<OpenIdServerModule>();

            var app = builder.Build();

            app.InitializeApplication();
            app.Lifetime.ApplicationStopping.Register(IocContext.Instance.Dispose);

            app.Run();
        }
        catch (Exception e)
        {
            logger.LogError(e, e.Message);
            throw;
        }
        finally
        {
            factory.Dispose();
        }
    }
}