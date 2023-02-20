using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

using Volo.Abp.Domain.Entities;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Domain.Repositories.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore;

using XCloud.Database.EntityFrameworkCore.Extensions;

namespace XCloud.Database.EntityFrameworkCore.Repository;

public interface IEfRepository<T> : IRepository<T>, IEfCoreRepository<T>, IDisposable where T : class, IEntity
{
    Task InsertNowAsync(T model, CancellationToken? cancellationToken = default);
    
    Task DeleteNowAsync(Expression<Func<T, bool>> where, CancellationToken? cancellationToken = default);
    
    Task DeleteNowAsync(T model, CancellationToken? cancellationToken = default);
    
    Task UpdateNowAsync(T model, CancellationToken? cancellationToken = default);

    Task<T[]> QueryManyAsync(Expression<Func<T, bool>> where, int? count = null,
        CancellationToken? cancellationToken = default);

    Task<T> QueryOneAsync(Expression<Func<T, bool>> where, CancellationToken? cancellationToken = default);

    Task<T[]> QueryManyAsNoTrackingAsync(Expression<Func<T, bool>> where, int? count = null,
        CancellationToken? cancellationToken = default);
}

public abstract class EfRepositoryBase<DbContextType, T> : EfCoreRepository<DbContextType, T>, IEfRepository<T>
    where DbContextType : DbContext, IEfCoreDbContext
    where T : class, IEntity
{
    protected EfRepositoryBase(IServiceProvider serviceProvider) : 
        base(serviceProvider.GetRequiredService<IDbContextProvider<DbContextType>>())
    {
        this.ServiceProvider = serviceProvider;
    }

    private CancellationToken GetCancellationTokenOrDefault(CancellationToken? token) =>
        token ?? this.CancellationTokenProvider.Token;
    
    public virtual async Task InsertNowAsync(T model, CancellationToken? cancellationToken = default)
    {
        var db = await this.GetDbContextAsync();

        var set = db.Set<T>();

        set.AddRange(new[] { model });

        await db.SaveChangesAsync(this.GetCancellationTokenOrDefault(cancellationToken));
    }

    public virtual async Task DeleteNowAsync(Expression<Func<T, bool>> where, CancellationToken? cancellationToken = default)
    {
        var db = await this.GetDbContextAsync();

        var set = db.Set<T>();

        set.RemoveRange(set.Where(where));

        await db.SaveChangesAsync(this.GetCancellationTokenOrDefault(cancellationToken));
    }

    public virtual async Task DeleteNowAsync(T model, CancellationToken? cancellationToken = default)
    {
        var db = await this.GetDbContextAsync();

        var entry = db.AttachEntityIfNot(model);

        if (entry.State != EntityState.Deleted)
        {
            entry.State = EntityState.Deleted;
        }

        await db.SaveChangesAsync(this.GetCancellationTokenOrDefault(cancellationToken));
    }

    public virtual async Task UpdateNowAsync(T model, CancellationToken? cancellationToken = default)
    {
        var db = await this.GetDbContextAsync();

        var entry = db.AttachEntityIfNot(model);

        if (entry.State != EntityState.Modified && !entry.Properties.Any(x => x.IsModified))
        {
            entry.State = EntityState.Modified;
        }

        await db.SaveChangesAsync(this.GetCancellationTokenOrDefault(cancellationToken));
    }


    private async Task<T[]> QueryManyImplAsync(IQueryable<T> query, 
        Expression<Func<T, bool>> where, int? count = null,
        CancellationToken? cancellationToken = default)
    {
        query = query.Where(where);

        if (count != null)
        {
            query = query.Take(count.Value);
        }

        var res = await query.ToArrayAsync(this.GetCancellationTokenOrDefault(cancellationToken));

        return res;
    }

    public async Task<T[]> QueryManyAsNoTrackingAsync(Expression<Func<T, bool>> where, int? count = null,
        CancellationToken? cancellationToken = default)
    {
        var db = await this.GetDbContextAsync();

        var query = db.Set<T>().AsNoTracking();

        return await QueryManyImplAsync(query, where, count, cancellationToken);
    }

    public async Task<T[]> QueryManyAsync(Expression<Func<T, bool>> where, int? count = null,
        CancellationToken? cancellationToken = default)
    {
        var db = await this.GetDbContextAsync();

        var query = db.Set<T>().AsTracking();

        return await QueryManyImplAsync(query, where, count, cancellationToken);
    }

    public virtual async Task<T> QueryOneAsync(Expression<Func<T, bool>> where, CancellationToken? cancellationToken = default)
    {
        var query = await this.GetQueryableAsync();

        query = query.Where(where);

        var res = await query.FirstOrDefaultAsync(this.GetCancellationTokenOrDefault(cancellationToken));

        return res;
    }

    public virtual void Dispose() { }
}