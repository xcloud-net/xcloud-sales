# linq2db

```xml
	<ItemGroup>
		<PackageReference Include="linq2db" Version="3.1.3" />
	</ItemGroup>
```

### fluent mapping

https://github.com/linq2db/linq2db/issues/1160
https://github.com/linq2db/linq2db/issues/1089

```csharp
    public class Context : DataConnection
    {
        public Context() : base(LinqToDB.ProviderName.SqlServer2012, "connection string")
        {
            if (mappingSchema == null)
                mappingSchema = InitContextMappings(this.MappingSchema);
        }

        public ITable<User> Users => GetTable<User>();
        public ITable<UserAddress> UserAddress => GetTable<UserAddress>();

        private static MappingSchema mappingSchema;

        private static MappingSchema InitContextMappings(MappingSchema ms)
        {
            ms.GetFluentMappingBuilder()
                .Entity<User>()
                .HasTableName("Users")
                .HasPrimaryKey(x => x.UserId).HasIdentity(x => x.UserId);

            ms.GetFluentMappingBuilder()
                .Entity<UserAddress>()
                .HasTableName("UsersAddresses")
                .HasPrimaryKey(x => new { x.UserId, x.AddressId })
                .Association<User>(x => x.User, (y, z) => y.UserId == z.UserId);

            return ms;
        }
    }
```

### repository

```csharp
using FluentAssertions;
using XCloud.Core.Extension;
using LinqToDB;
using LinqToDB.Configuration;
using LinqToDB.Data;
using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using XCloud.Database.Abstractions;
using XCloud.Core.Ddd.Entity;

namespace XCloud.Database.Linq2DB
{
#if DEBUG
    class dd
    {
        dd()
        {
            var builder = new LinqToDbConnectionOptionsBuilder();
            builder.UseMySql("");
            var option = builder.Build();

            using var con = new DataConnection(option);

            using var t = con.BeginTransaction();

            t.Commit();
            t.Rollback();
        }
    }
#endif
    public interface ILinq2DBRepository<T> : ILinqRepository<T> where T : class, IEntityBase
    {
        DataConnection Database { get; }
    }
    public abstract partial class Linq2DBRepositoryBase<T> : ILinq2DBRepository<T> where T : class, IEntityBase
    {
        private readonly Lazy<DataConnection> lazy_db;

        protected Linq2DBRepositoryBase(Func<DataConnection> db_getter)
        {
            this.lazy_db = new Lazy<DataConnection>(db_getter);
        }

        public DataConnection Database => this.lazy_db.Value;
        public IQueryable<T> Queryable => this.Database.GetTable<T>();

        public virtual void Dispose()
        {
            if (this.lazy_db.IsValueCreated)
                this.lazy_db.Value?.Dispose();
        }

        public int Insert(T model)
        {
            model.Should().NotBeNull();
            model.Id.Should().NotBeNullOrEmpty();

            return this.Database.Insert(model);
        }

        public Task<int> InsertAsync(T model)
        {
            model.Should().NotBeNull();
            model.Id.Should().NotBeNullOrEmpty();

            return this.Database.InsertAsync(model);
        }

        public int Delete(T model)
        {
            model.Should().NotBeNull();
            model.Id.Should().NotBeNullOrEmpty();

            return this.DeleteWhere(x => x.Id == model.Id);
        }

        public Task<int> DeleteAsync(T model)
        {
            model.Should().NotBeNull();
            model.Id.Should().NotBeNullOrEmpty();

            return this.DeleteWhereAsync(x => x.Id == model.Id);
        }

        public int DeleteWhere(Expression<Func<T, bool>> where)
        {
            where.Should().NotBeNull();

            return this.Database.Delete(where);
        }

        public Task<int> DeleteWhereAsync(Expression<Func<T, bool>> where)
        {
            where.Should().NotBeNull();

            return this.Database.DeleteAsync(where);
        }

        public bool Exist(Expression<Func<T, bool>> where)
        {
            return this.Queryable.WhereIfNotNull(where).Any();
        }

        public Task<bool> ExistAsync(Expression<Func<T, bool>> where)
        {
            return this.Queryable.WhereIfNotNull(where).AnyAsync();
        }

        public int QueryCount(Expression<Func<T, bool>> where)
        {
            return this.Queryable.WhereIfNotNull(where).Count();
        }

        public Task<int> QueryCountAsync(Expression<Func<T, bool>> where)
        {
            return this.Queryable.WhereIfNotNull(where).CountAsync();
        }

        public T QueryOne(Expression<Func<T, bool>> where)
        {
            return this.Queryable.WhereIfNotNull(where).FirstOrDefault();
        }

        public Task<T> QueryOneAsync(Expression<Func<T, bool>> where)
        {
            return this.Queryable.WhereIfNotNull(where).FirstOrDefaultAsync();
        }

        public T[] QueryMany(Expression<Func<T, bool>> where, int? count = null)
        {
            return this.QueryMany<object>(where: where, count: count);
        }

        public Task<T[]> QueryManyAsync(Expression<Func<T, bool>> where, int? count = null)
        {
            return this.QueryManyAsync<object>(where: where, count: count);
        }

        public int Update(T model)
        {
            model.Should().NotBeNull();
            model.Id.Should().NotBeNullOrEmpty();

            //this.Database.GetTable<T>().Where(x => x.Id == model.Id).Set(x => x.CreateTimeUtc, DateTime.UtcNow).Update();

            return this.Database.Update(model);
        }

        public Task<int> UpdateAsync(T model)
        {
            model.Should().NotBeNull();
            model.Id.Should().NotBeNullOrEmpty();

            return this.Database.UpdateAsync(model);
        }

        IQueryable<T> __query_many__<OrderByColumn>(Expression<Func<T, bool>> where, int? count = null, int? skip = null, Expression<Func<T, OrderByColumn>> order_by = null, bool desc = true)
        {
            var query = this.Queryable;
            query = query.WhereIfNotNull(where);
            if (order_by != null)
            {
                if (desc)
                    query = query.OrderByDescending(order_by);
                else
                    query = query.OrderBy(order_by);
            }
            if (skip != null)
            {
                order_by.Should().NotBeNull("必须先排序");
                query = query.Skip(skip.Value);
            }
            if (count != null)
            {
                query = query.Take(count.Value);
            }
            return query;
        }

        public T[] QueryMany<OrderByColumn>(Expression<Func<T, bool>> where, int? count = null, int? skip = null, Expression<Func<T, OrderByColumn>> order_by = null, bool desc = true)
        {
            var query = this.__query_many__(where, count, skip, order_by, desc);
            var res = query.ToArray();

            return res;
        }

        public async Task<T[]> QueryManyAsync<OrderByColumn>(Expression<Func<T, bool>> where, int? count = null, int? skip = null, Expression<Func<T, OrderByColumn>> order_by = null, bool desc = true)
        {
            var query = this.__query_many__(where, count, skip, order_by, desc);
            var res = await query.ToArrayAsync();

            return res;
        }
    }
}
```