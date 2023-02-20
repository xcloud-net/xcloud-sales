using XCloud.Core.Helper;

namespace XCloud.Sales.Core;

public interface ISalesBaseEntity : IEntity, IDbTableFinder
{
    //
}

/// <summary>
/// Base class for entities
/// 这里计划把id都改为string，但是工程量巨大，只完成了部分。
/// 所以出现了string和int混用的情况
///
/// 都说自增int性能好，但是我有不同看法。这种对比是基于自增int和无序guid进行的。
/// 我这里使用string的原因有两个：
/// 1.我使用的是有序guid，索引性能好
/// 2.不使用数据库自增有利于后期分布式扩展
/// 3.数据库外生成id可以在不提交数据库事务的情况下就使用id
///
/// </summary>
public abstract class SalesBaseEntity : SalesBaseEntity<int>
{
    //
}

public abstract class SalesBaseEntity<TKey> : ISalesBaseEntity, IEntity<TKey>
{
    /// <summary>
    /// Gets or sets the entity identifier
    /// </summary>
    public virtual TKey Id { get; set; }

    public object[] GetKeys()
    {
        return new object[] { Id };
    }
}