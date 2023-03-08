using FluentAssertions;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Domain.Entities;
using XCloud.Core.Application;
using XCloud.Core.Dto;
using XCloud.Core.Extension;
using XCloud.Database.EntityFrameworkCore.Mapping;

namespace XCloud.Database.EntityFrameworkCore.Extensions;

public static class EntityFrameworkExtension
{
    public static async Task UpdateDataCollectionAsync<T, TKey>(this DbContext db,
        T[] data, Func<T, TKey> keySelector, Expression<Func<T, bool>> where,
        Func<T, Task<T>> entityInitFunc = null)
        where T : class
    {
        var set = db.Set<T>();
        var dataOrigin = await set.Where(where).ToArrayAsync();

        //需要删除的
        var deletedEntities = dataOrigin.NotInBy(data, keySelector).ToArray();
        if (deletedEntities.Any())
        {
            set.RemoveRange(deletedEntities);
        }

        //需要添加的
        var addedEntities = data.NotInBy(dataOrigin, keySelector).ToArray();
        if (addedEntities.Any())
        {
            var initionFunc = entityInitFunc ?? (x => Task.FromResult(x));

            addedEntities = await Task.WhenAll(addedEntities.Select(x => initionFunc.Invoke(x)));

            set.AddRange(addedEntities);
        }

        await db.SaveChangesAsync();
    }

    [Obsolete]
    public static async Task<IEnumerable<D>> AttachData<D, T>(this DbContext db,
        IEnumerable<D> source,
        Func<D, string> getIdFunction,
        Action<D, T[]> callbackAction) where T : class, IEntityBase
    {
        var sourceArray = source.ToArray();
        if (!sourceArray.Any())
        {
            return sourceArray;
        }
        var ids = sourceArray.Select(getIdFunction).Distinct().ToArray();
        var alldata = await db.Set<T>().AsNoTracking().Where(x => ids.Contains(x.Id)).ToArrayAsync();

        foreach (var m in sourceArray)
        {
            var items = alldata.Where(x => getIdFunction.Invoke(m) == x.Id).ToArray();
            callbackAction.Invoke(m, items);
        }

        return sourceArray;
    }

    public static async Task<IEnumerable<D>> AttachItems<D, T>(this DbContext db,
        IEnumerable<D> source,
        Expression<Func<T, string>> getFieldIdExpression,
        Action<D, T[]> callbackAction)
        where D : class, IEntityBase
        where T : class, IEntityBase
    {
        var sourceArray = source.ToArray();
        if (!sourceArray.Any())
        {
            return sourceArray;
        }

        var ids = sourceArray.Select(x => x.Id).Distinct().ToList();

        (ids.GetType() == typeof(List<string>)).Should().BeTrue();
        var containsMethod = typeof(List<string>).GetMethods()
            .Where(x => x.Name == nameof(ids.Contains))
            .Where(x => x.GetParameters().Length == 1)
            .Where(x => x.GetParameters().FirstOrDefault()?.ParameterType == typeof(string))
            .FirstOrDefault();

        if (containsMethod == null)
            throw new AbpException(nameof(containsMethod));

        var memberExpression = getFieldIdExpression.Body as MemberExpression;
        if (memberExpression == null)
            throw new AbpException(nameof(memberExpression));

        var propertyInfo = memberExpression.Member as PropertyInfo;
        if (propertyInfo == null)
            throw new AbpException(nameof(propertyInfo));

        //ids.Contains("");

        //x
        var parameterExpr = Expression.Parameter(typeof(T), "x");
        //ids
        var idsExpr = Expression.Constant(ids);
        //x.XXId
        MemberExpression fieldSelectorExpr = Expression.Property(expression: parameterExpr, property: propertyInfo);
        //Contains(ids,x.XXId)
        var methodCallExpr = Expression.Call(instance: idsExpr, method: containsMethod,
            arguments: new Expression[] { fieldSelectorExpr });
        //x => ids.Contains(x.XXId)
        var expression = Expression.Lambda<Func<T, bool>>(methodCallExpr,
            parameters: new ParameterExpression[] { parameterExpr });

        var alldata = await db.Set<T>().AsNoTracking().Where(expression).ToArrayAsync();

        var fieldSelector = getFieldIdExpression.Compile();

        foreach (var m in sourceArray)
        {
            var items = alldata.Where(x => fieldSelector.Invoke(x) == m.Id).ToArray();
            callbackAction.Invoke(m, items);
        }

        return sourceArray;
    }

    public static Type[] FindEntityTypes(this DbContext db)
    {
        var res = db
            .GetType()
            .GetProperties()
            .Select(x => x.PropertyType)
            .Where(x => x.IsGenericType_(typeof(DbSet<>)))
            .Select(x => x.GenericTypeArguments.FirstOrDefault())
            .WhereNotNull()
            .ToArray();

        return res;
    }

    public static async Task<int> TrySaveChangesAsync(this DbContext db)
    {
        if (db.ChangeTracker.HasChanges())
        {
            var effected = await db.SaveChangesAsync();
            return effected;
        }
        return default;
    }

    /// <summary>
    /// 如果存在未提交的更改就抛出异常
    /// 为了防止不可预测的提交
    /// </summary>
    public static void ThrowIfHasChanges(this DbContext context)
    {
        if (context.ChangeTracker.HasChanges())
        {
            throw new UnSubmitChangesException($"{context.GetType().FullName}存在未提交的更改");
        }
    }

    public static void RollbackEntityChanges(this DbContext context)
    {
        if (context.ChangeTracker.HasChanges())
        {
            var changedState = new[]
            {
                //EntityState.Unchanged,
                //EntityState.Detached,
                EntityState.Added,
                EntityState.Modified,
                EntityState.Deleted
            };

            var entries = context.ChangeTracker.Entries().Where(e => changedState.Contains(e.State)).ToArray();

            foreach (var m in entries)
            {
                m.State = EntityState.Unchanged;
            }
        }
    }

    /// <summary>
    /// 把实体加载到内存上下文中
    /// </summary>
    public static EntityEntry<T> AttachEntityWithStringPkIfNot<T>(this DbContext db, T entity) where T : class, IEntityBase
    {
        entity.Should().NotBeNull();
        entity.Id.Should().NotBeNullOrEmpty();

        var set = db.Set<T>();

        var entry = db.ChangeTracker.Entries<T>().FirstOrDefault(ent => ent.Entity.Id == entity.Id);

        if (entry == null)
        {
            entry = set.Attach(entity);
        }
        return entry;
    }

    public static EntityEntry<T> AttachEntityIfNot<T>(this DbContext db, T entity) where T : class, IEntity
    {
        var set = db.Set<T>();

        var entry = db.ChangeTracker.Entries<T>().FirstOrDefault(ent => ent.Entity.GetKeys().SequenceEqual(entity.GetKeys()));

        if (entry == null)
        {
            entry = set.Attach(entity);
        }
        return entry;
    }

    /// <summary>
    /// 自动分页
    /// </summary>
    public static async Task<PagedResponse<T>> ToPagedListAsync<T, SortColumn>(this IQueryable<T> query,
        int page, int pagesize,
        Expression<Func<T, SortColumn>> orderby, bool desc = true)
    {
        var data = new PagedResponse<T>(page, pagesize);

        data.TotalCount = await query.CountAsync();

        var sortedQuery = desc ? query.OrderByDescending(orderby) : query.OrderBy(orderby);

        data.Items = await sortedQuery.QueryPage(page, pagesize).ToArrayAsync();

        return data;
    }

    public static void ConfigEntityMapperFromAssemblies<TableType>(this ModelBuilder modelBuilder, params Assembly[] assemblies)
    {
        modelBuilder.Should().NotBeNull();
        assemblies.Should().NotBeNullOrEmpty();

        var allTypes = assemblies.GetAllTypes().Where(x => x.IsNormalPublicClass()).ToArray();

        foreach (var cls in allTypes)
        {
            if (!cls.IsAssignableTo_<IMappingConfiguration>())
                continue;

            var configType = cls.GetInterfaces().FirstOrDefault(x => x.IsGenericType_(typeof(IEntityTypeConfiguration<>)));
            if (configType == null)
                continue;

            var entityType = configType.GetGenericArguments().FirstOrDefault();
            if (entityType == null || !entityType.IsAssignableTo_<TableType>())
                continue;

            var configuration = (IMappingConfiguration)Activator.CreateInstance(cls);
            if (configuration == null)
                throw new AbpException(nameof(ConfigEntityMapperFromAssemblies));
            
            configuration.ApplyConfiguration(modelBuilder);
        }
    }

}