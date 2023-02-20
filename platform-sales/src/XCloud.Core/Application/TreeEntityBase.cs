namespace XCloud.Core.Application;

public interface ITreeEntityBase
{
    string ParentId { get; set; }
    string TreeGroupKey { get; set; }
}

/// <summary>
/// 树结构
/// </summary>
public abstract class TreeEntityBase : EntityBase, ITreeEntityBase
{
    protected TreeEntityBase() { }

    /// <summary>
    /// 父级Id
    /// </summary>
    public virtual string ParentId { get; set; }

    /// <summary>
    /// 分组
    /// </summary>
    public virtual string TreeGroupKey { get; set; }
}