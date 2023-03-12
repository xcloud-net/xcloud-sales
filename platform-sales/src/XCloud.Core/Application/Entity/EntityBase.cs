using Volo.Abp.Auditing;
using Volo.Abp.Domain.Entities;
using XCloud.Core.Helper;

namespace XCloud.Core.Application.Entity;

public interface IEntityBase : IEntity<string>, IDbTableFinder
{
    new string Id { get; set; }

    //DateTime CreationTime { get; set; }
}

/// <summary>
/// 实体基类
/// </summary>
public abstract class EntityBase : IEntityBase, IHasCreationTime
{
    protected EntityBase()
    {
        //
    }

    public virtual string Id { get; set; }

    public virtual DateTime CreationTime { get; set; }

    public virtual object[] GetKeys()
    {
        return new object[] { this.Id };
    }
}