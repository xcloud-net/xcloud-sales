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
using Volo.Abp;
using XCloud.AspNetMvc.Configuration;
using XCloud.Platform.Api;
using XCloud.Platform.Test.Service;

namespace XCloud.Platform.Test;

[TestClass]
public abstract class TestBase
{
    private string GetRequiredRootPath()
    {
        var path = Path.Combine(Assembly.GetExecutingAssembly().Location,
            "../../../../../",
            typeof(PlatformApiModule).Assembly.GetName().Name ?? string.Empty);

        if (!Directory.Exists(path))
            throw new AbpException(nameof(GetRequiredRootPath));

        return path;
    }

    protected WebApplication WebApplication;

    [TestInitialize]
    public void TestInit()
    {
        var path = this.GetRequiredRootPath();

        var builder = WebApplication.CreateBuilder(new WebApplicationOptions()
        {
            ContentRootPath = path,
            WebRootPath = Path.Combine(path, "wwwroot")
        });

        builder.Host.UseAutofac();
        builder.WebHost.UseTestServer();

        builder.TrySettleUpConfig(configBuilder =>
        {
            configBuilder.AddJsonFile(
                Path.Combine(path, "appsettings.json"),
                optional: false);
        });

        builder.Services.AddApplication<PlatformTestModule>();

        var server = builder.Build();

        server.InitializeApplication();

        server.Start();

        this.WebApplication = server;
    }

    [TestCleanup]
    public async Task TestCleanup()
    {
        if (this.WebApplication != null)
        {
            await this.WebApplication.StopAsync();
            using (this.WebApplication)
            {
                //
            }
        }
    }

    //[TestMethod]
    public void TestMethod1()
    {
        var builder = WebHost.CreateDefaultBuilder();

        builder.UseTestServer();

        builder.ConfigureAppConfiguration((context, configurationBuilder) =>
        {
            //
        });

        builder.ConfigureServices(services => { services.AddSingleton<TestUserAccountService>(); });

        builder.Configure(app =>
        {
            //
        });

        using var server = builder.Build();

        server.Start();

        using var s = server.Services.CreateScope();

        s.ServiceProvider.GetRequiredService<TestUserAccountService>();
    }
}