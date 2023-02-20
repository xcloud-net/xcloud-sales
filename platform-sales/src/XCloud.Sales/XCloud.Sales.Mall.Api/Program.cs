using Autofac;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Hosting;
using XCloud.AspNetMvc.Configuration;
using XCloud.Core.DependencyInjection;
using XCloud.Logging;

namespace XCloud.Sales.Mall.Api;

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
            builder.Host.ConfigureContainer<ContainerBuilder>(containerBuilder =>
            {
                //builder.RegisterModule(new AutofacModuleRegister());
                //use autofac api
            });

            builder.TrySettleUpConfig(configBuilder => { });
            
            builder.Services.AddApplication<SalesMallApiModule>();

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