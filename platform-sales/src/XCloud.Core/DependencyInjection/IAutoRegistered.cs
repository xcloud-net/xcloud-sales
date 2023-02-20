using XCloud.Core.Helper;

namespace XCloud.Core.DependencyInjection;

/// <summary>
/// autofac 自动查找注册，
/// 默认使用瞬时生命周期，可以使用attribute来指定
/// </summary>
public interface IAutoRegistered : IFinder { }

public interface ITransitInstance : IAutoRegistered { }

public interface ISingleInstance : IAutoRegistered { }

public interface IScopedInstance : IAutoRegistered { }

public class AutoRegisterdConfigurationAttribute : Attribute
{
    public bool Replace { get; set; } = false;
    public bool TryAdd { get; set; } = false;
    public bool Ignore { get; set; } = false;
}