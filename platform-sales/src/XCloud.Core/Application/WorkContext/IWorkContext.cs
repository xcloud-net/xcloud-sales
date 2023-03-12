using Microsoft.Extensions.Configuration;
using Volo.Abp.DependencyInjection;
using XCloud.Core.Json;

namespace XCloud.Core.Application.WorkContext;

public interface IWorkContext<T> : IWorkContext { }

/// <summary>
/// 在业务代码中使用，不要在底层基础框架中使用，不然容易导致循环依赖
/// </summary>
public interface IWorkContext : IDisposable
{
    AppConfig AppConfig { get; }
    IServiceProvider ServiceProvider { get; }
    IConfiguration Configuration { get; }
    ILogger Logger { get; }
    IJsonDataSerializer JsonSerializer { get; }
}

internal class DefaultWorkContext<T> : IWorkContext<T>
{
    public IServiceProvider ServiceProvider { get; }
    public IAbpLazyServiceProvider AbpLazyServiceProvider { get; }
    public ILogger Logger { get; }
        
    //lazy resolve
    TService LazyService<TService>() => this.AbpLazyServiceProvider.LazyGetRequiredService<TService>();
        
    public AppConfig AppConfig => this.LazyService<AppConfig>();

    public IConfiguration Configuration => this.LazyService<IConfiguration>();

    public IJsonDataSerializer JsonSerializer => this.LazyService<IJsonDataSerializer>();

    public DefaultWorkContext(IServiceProvider provider)
    {
        this.ServiceProvider = provider;
        this.AbpLazyServiceProvider = provider.GetRequiredService<IAbpLazyServiceProvider>();
        this.Logger = provider.GetRequiredService<ILogger<T>>();
    }

    public void Dispose()
    {
        //
    }
}