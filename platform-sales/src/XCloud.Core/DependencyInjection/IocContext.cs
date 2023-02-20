using XCloud.Core.DependencyInjection.Extension;

namespace XCloud.Core.DependencyInjection;

/// <summary>
/// 依赖注入对象管理容器
/// </summary>
public class IocContext : IDisposable
{
    public static readonly IocContext Instance = new IocContext();

    private IServiceProvider Root { get; set; }

    public IServiceProvider Container => this.Root ?? throw new Exception("没有设置依赖注入容器");

    /// <summary>
    /// 是否初始化
    /// </summary>
    public bool Inited => this.Root != null;

    public IocContext SetRootContainer(IServiceProvider root)
    {
        this.Root = root;
        return this;
    }

    /// <summary>
    /// 销毁所有组件
    /// </summary>
    public void Dispose()
    {
        if (!this.Inited)
            return;

        this.Container.DisposeSingleInstanceService();

        /*
        if (this.Container is ServiceProvider)
            ((ServiceProvider)this.Container).Dispose();
            */
    }

    ~IocContext()
    {
        System.Diagnostics.Debug.WriteLine($"{nameof(IocContext)},collected");
    }
}