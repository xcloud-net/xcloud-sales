using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using XCloud.Core.Application;
using XCloud.Database.EntityFrameworkCore.Extensions;

namespace XCloud.Database.EntityFrameworkCore.Crud.Helper;

/// <summary>
/// 还是先查出来再更新，没有hack ef
/// </summary>
/// <typeparam name="T"></typeparam>
public interface IEfUpdate<T>
{
    /// <summary>
    /// 忽略全局过滤器
    /// </summary>
    IEfUpdate<T> IgnoreFilter();

    /// <summary>
    /// 还原全局过滤器
    /// </summary>
    /// <returns></returns>
    IEfUpdate<T> RestoreFilter();

    /// <summary>
    /// 查询条件
    /// </summary>
    IEfUpdate<T> Where(Expression<Func<T, bool>> condition);

    /// <summary>
    /// 设置字段
    /// </summary>
    IEfUpdate<T> SetField<F>(Expression<Func<T, F>> target, F value);

    /// <summary>
    /// 修改
    /// </summary>
    IEfUpdate<T> SetEntity(Action<T> action);

    /// <summary>
    /// 设置实体，按照相同名称的字段匹配
    /// </summary>
    IEfUpdate<T> SetEntity(object value);

    /// <summary>
    /// 清空所有条件数据和修改
    /// </summary>
    IEfUpdate<T> ClearAll();

    /// <summary>
    /// 执行更新
    /// </summary>
    Task<int> ExecuteAsync();
}

public class EfUpdate<T> : IEfUpdate<T> where T : class, IEntityBase
{
    private readonly DbContext _db;

    private readonly int _maxBatchSize = 1000;

    public EfUpdate(DbContext db, int? maxBatchSize = null)
    {
        db.Should().NotBeNull();
        db.ThrowIfHasChanges();
        this._db = db;
        if (maxBatchSize != null)
        {
            this._maxBatchSize = maxBatchSize.Value;
        }

        (this._maxBatchSize > 1).Should().BeTrue();
    }

    private bool? _ignoreFilter = null;

    public IEfUpdate<T> IgnoreFilter()
    {
        this._ignoreFilter = true;
        return this;
    }

    public IEfUpdate<T> RestoreFilter()
    {
        this._ignoreFilter = null;
        return this;
    }

    private readonly List<Expression<Func<T, bool>>> _conditions = new List<Expression<Func<T, bool>>>();

    public IEfUpdate<T> Where(Expression<Func<T, bool>> condition)
    {
        condition.Should().NotBeNull();
        this._conditions.Add(condition);
        return this;
    }

    private readonly List<Action<T>> _updators = new();

    public IEfUpdate<T> SetField<F>(Expression<Func<T, F>> target, F value)
    {
        target.Should().NotBeNull();

        this._updators.Add((T x) => x.SetFields(target, value));

        return this;
    }

    public IEfUpdate<T> SetEntity(Action<T> action)
    {
        action.Should().NotBeNull();

        this._updators.Add((T x) => action.Invoke(x));

        return this;
    }

    public IEfUpdate<T> SetEntity(object value)
    {
        value.Should().NotBeNull();

        this._updators.Add((T x) => x.SetEntityFields(value));

        return this;
    }

    public IEfUpdate<T> ClearAll()
    {
        this._ignoreFilter = null;
        this._conditions.Clear();
        this._updators.Clear();
        return this;
    }

    public async Task<int> ExecuteAsync()
    {
        this._conditions.Should().NotBeNullOrEmpty();
        this._updators.Should().NotBeNullOrEmpty();

        var db = this._db;
        db.ThrowIfHasChanges();

        var set = db.Set<T>();

        var query = set.AsTracking();
        //global filter
        if (this._ignoreFilter != null && this._ignoreFilter.Value == true)
        {
            query = query.IgnoreQueryFilters();
        }

        //where conditions
        foreach (var condition in this._conditions)
        {
            query = query.Where(condition);
        }

        //query data
        var data = await query.Take(this._maxBatchSize).ToArrayAsync();

        if (data.Any())
        {
            (data.Length < this._maxBatchSize).Should().BeTrue("满足当前条件的记录数过多，请精确查询条件再试");
            //modify data
            foreach (var m in data)
            {
                var model = m;
                foreach (var updator in this._updators)
                {
                    updator.Invoke(model);
                }
            }

            //update data
            if (db.ChangeTracker.HasChanges())
            {
                var res = await db.SaveChangesAsync();

                db.RollbackEntityChanges();
                return res;
            }
        }

        return 0;
    }
}