```c#
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Domain.Repositories.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore;
using XCloud.Core.Ddd.Entity;
using XCloud.Core.Ddd.Repository;
using XCloud.Database.EntityFrameworkCore.Extensions;

namespace XCloud.Database.EntityFrameworkCore.Repository
{
    public interface IEFRepository<T> : IMyRepository<T>, IEfCoreRepository<T, string> where T : class, IEntityBase
    {
        Task<IQueryable<Table>> GetQuerybleWithoutGlobalFilterAsync<Table>() where Table : class;
    }

    /// <summary>
    /// 永远不会修改iid，uid，create time
    /// </summary>
    public abstract class EFRepositoryBase<T, DbContextType> : EfCoreRepository<DbContextType, T, string>, IEFRepository<T>
        where T : class, IEntityBase
        where DbContextType : DbContext, IEfCoreDbContext
    {
        protected IEntityHelper EntityHelper => this.LazyServiceProvider.LazyGetRequiredService<IEntityHelper>();

        protected EFRepositoryBase(IServiceProvider serviceProvider) : this(serviceProvider, serviceProvider.GetRequiredService<IDbContextProvider<DbContextType>>())
        {
            //
        }

        protected EFRepositoryBase(IServiceProvider serviceProvider, IDbContextProvider<DbContextType> dbContextProvider) : base(dbContextProvider)
        {
            this.ServiceProvider = serviceProvider;
        }

        public async Task<IQueryable<Table>> GetQuerybleWithoutGlobalFilterAsync<Table>() where Table : class
        {
            var db = await this.GetDbContextAsync();

            var query = db.Set<Table>().IgnoreQueryFilters();

            return query;
        }

        protected CancellationToken __token__(CancellationToken? token) => token ?? this.CancellationTokenProvider.Token;

        public virtual async Task<int> InsertAsync(T model, CancellationToken? cancellationToken = default)
        {
            var db = await this.GetDbContextAsync();

            var set = db.Set<T>();

            set.AddRange(new[] { model });

            var res = await db.SaveChangesAsync(this.__token__(cancellationToken));

            return res;
        }

        public virtual async Task<int> DeleteWhereAsync(Expression<Func<T, bool>> where, CancellationToken? cancellationToken = default)
        {
            var db = await this.GetDbContextAsync();

            var set = db.Set<T>();

            set.RemoveRange(set.Where(where));

            var res = await db.SaveChangesAsync(this.__token__(cancellationToken));

            return res;
        }

        public virtual async Task<int> UpdateAsync(T model, CancellationToken? cancellationToken = default)
        {
            model.Id.Should().NotBeNullOrEmpty("必须包含主键");

            var db = await this.GetDbContextAsync();

            var entry = db.AttachIfNot(model);

            if (entry.State != EntityState.Modified && !entry.Properties.Any(x => x.IsModified))
            {
                entry.State = EntityState.Modified;
            }

            entry.Property(x => x.Id).IsModified = false;
            entry.Property(x => x.CreationTime).IsModified = false;

            var res = await db.SaveChangesAsync(this.__token__(cancellationToken));

            return res;
        }

        #region 查询

        public async Task<T[]> QueryManyAsync(Expression<Func<T, bool>> where, int? count = null,
            CancellationToken? cancellationToken = default)
        {
            var db = await this.GetDbContextAsync();

            var query = db.Set<T>().AsNoTracking();

            query = query.Where(where);

            if (count != null)
            {
                query = query.Take(count.Value);
            }

            var res = await query.ToArrayAsync(this.__token__(cancellationToken));

            return res;
        }

        public virtual async Task<T> QueryOneAsync(Expression<Func<T, bool>> where, CancellationToken? cancellationToken = default)
        {
            var query = await this.GetQueryableAsync();

            query = query.Where(where);

            var res = await query.FirstOrDefaultAsync(this.__token__(cancellationToken));

            return res;
        }

        public virtual async Task<int> QueryCountAsync(Expression<Func<T, bool>> where, CancellationToken? cancellationToken = default)
        {
            var db = await this.GetDbContextAsync();

            var query = db.Set<T>().AsNoTracking();

            query = query.Where(where);

            var res = await query.CountAsync(this.__token__(cancellationToken));

            return res;
        }

        public virtual async Task<bool> ExistAsync(Expression<Func<T, bool>> where, CancellationToken? cancellationToken = default)
        {
            var db = await this.GetDbContextAsync();

            var query = db.Set<T>().AsNoTracking();

            query = query.Where(where);

            var res = await query.AnyAsync(this.__token__(cancellationToken));

            return res;
        }
        #endregion

        public virtual void Dispose() { }
    }
}

```
