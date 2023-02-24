using System;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using XCloud.AspNetMvc.Configuration;
using XCloud.Platform.Api;
using XCloud.Platform.Member.Application.Service.User;

namespace XCloud.Platform.Test;

public class MyClass
{
    public void Hello()
    {
        Console.WriteLine("hello world,integrated test");
    }
}

[TestClass]
public class UnitTest1
{
    [TestMethod]
    public void TestMethod1()
    {
        var builder = WebHost.CreateDefaultBuilder();

        builder.UseTestServer();

        builder.ConfigureAppConfiguration((context, configurationBuilder) =>
        {
            //
        });

        builder.ConfigureServices(services => { services.AddSingleton<MyClass>(); });

        builder.Configure(app =>
        {
            //
        });

        using var server = builder.Build();

        server.Start();

        using var s = server.Services.CreateScope();

        s.ServiceProvider.GetRequiredService<MyClass>().Hello();
    }

    [TestMethod]
    public async Task TestMethod2()
    {
        var builder = WebApplication.CreateBuilder();

        builder.Host.UseAutofac();
        builder.WebHost.UseTestServer();
        builder.Services.AddSingleton<MyClass>();

        builder.TrySettleUpConfig(configBuilder =>
        {
            configBuilder.AddJsonFile(
                Path.Combine(Assembly.GetExecutingAssembly().Location,
                    "../../../../../",
                    typeof(PlatformApiModule).Assembly.GetName().Name ?? string.Empty,
                    "appsettings.json"),
                optional: false);
        });

        builder.Services.AddApplication<PlatformApiModule>();

        using var server = builder.Build();

        server.InitializeApplication();

        server.Start();

        using var s = server.Services.CreateScope();

        s.ServiceProvider.GetRequiredService<MyClass>().Hello();
        var user = await s.ServiceProvider.GetRequiredService<IUserAccountService>().QueryUserAccountAsync("x");
    }
}