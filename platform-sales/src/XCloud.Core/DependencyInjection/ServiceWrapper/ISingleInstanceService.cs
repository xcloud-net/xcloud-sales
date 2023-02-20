namespace XCloud.Core.DependencyInjection.ServiceWrapper;

/// <summary>
/// 单利实现
/// </summary>
public interface ISingleInstanceService : IDisposable
{
    /// <summary>
    /// 数值越小，越先释放
    /// </summary>
    int DisposeOrder { get; }
}