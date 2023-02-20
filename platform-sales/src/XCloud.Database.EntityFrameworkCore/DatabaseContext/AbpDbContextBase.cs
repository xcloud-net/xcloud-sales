using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Volo.Abp.EntityFrameworkCore;
using System;
using Microsoft.Extensions.DependencyInjection;
using XCloud.Core.Helper;

namespace XCloud.Database.EntityFrameworkCore.DatabaseContext;

public abstract class AbpDbContextBase<DbContextImpl> : AbpDbContext<DbContextImpl> where DbContextImpl : DbContext
{
    private readonly ILoggerFactory loggerFactory;
    public AbpDbContextBase(IServiceProvider serviceProvider, DbContextOptions<DbContextImpl> option) : base(option)
    {
        this.loggerFactory = serviceProvider.GetRequiredService<ILoggerFactory>();
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (this.loggerFactory != null)
        {
            optionsBuilder.UseLoggerFactory(this.loggerFactory);
        }
    }

    /// <summary>
    /// 注册mapping
    /// </summary>
    /// <param name="modelBuilder"></param>
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        //
    }

    /// <summary>
    /// Attach an entity to the context or return an already attached entity (if it was already attached)
    /// 附加一个实体，如果已经存在就直接返回
    /// </summary>
    //protected virtual TEntity AttachEntityToContext<TEntity>(TEntity entity) where TEntity : ModelBase, new()
    //{
    //    //little hack here until Entity Framework really supports stored procedures
    //    //otherwise, navigation properties of loaded entities are not loaded until an entity is attached to the context
    //    var alreadyAttached = Set<TEntity>().Local.FirstOrDefault(x => x.Id == entity.Id);
    //    if (alreadyAttached == null)
    //    {
    //        //attach new entity
    //        Set<TEntity>().Attach(entity);
    //        return entity;
    //    }

    //    //entity is already loaded
    //    return alreadyAttached;
    //}

    /// <summary>
    /// 实体集合
    /// new的用法搜索 override new
    /// </summary>
    public new DbSet<T> Set<T>() where T : class, IDbTableFinder
    {
        return base.Set<T>();
    }

}