# free sql

https://github.com/dotnetcore/FreeSql

### fluent map api
```csharp
fsql.CodeFirst.Entity<Song>(eb =>
{
    eb.ToTable("song");
    eb.Ignore(a => a.Field1);
    eb.Property(a => a.Title).HasColumnType("varchar(50)").IsRequired();
    eb.Property(a => a.Url).HasMaxLength(100);

    eb.Property(a => a.RowVersion).IsRowVersion();
    eb.Property(a => a.CreateTime).HasDefaultValueSql("getdate()");

    eb.HasKey(a => a.Id);
    eb.HasIndex(a => a.Title).IsUnique().HasName("idx_xxx11");

    //一对多、多对一
    eb.HasOne(a => a.Type).HasForeignKey(a => a.TypeId).WithMany(a => a.Songs);

    //多对多
    eb.HasMany(a => a.Tags).WithMany(a => a.Songs, typeof(Song_tag));
});
fsql.GlobalFilter.Apply<test>("is-deleted", x => x.IsDeleted == 0);
```

```csharp
    public abstract class FreeSQLRepository<T> : IFreeSQLRepository<T> where T : class, IEntityBase
    {
        private readonly IFreeSql freeSql;
        protected FreeSQLRepository(IFreeSql freeSql) : this(() => freeSql)
        {
            //
        }

        protected FreeSQLRepository(Func<IFreeSql> func)
        {
            this.freeSql = func.Invoke();
        }

        public int Delete(T model)
        {
            model.Should().NotBeNull();
            model.Id.Should().NotBeNullOrEmpty();

            var res = this.DeleteWhere(x => x.Id == model.Id);

            return res;
        }

        protected CancellationToken __token__(CancellationToken? token) => token ?? CancellationToken.None;

        public async Task<int> DeleteAsync(T model, CancellationToken? cancellationToken = default)
        {
            model.Should().NotBeNull();
            model.Id.Should().NotBeNullOrEmpty();

            var res = await this.DeleteWhereAsync(x => x.Id == model.Id, cancellationToken);

            return res;
        }

        public int DeleteWhere(Expression<Func<T, bool>> where)
        {
            where.Should().NotBeNull();

            var res = this.freeSql.Delete<T>().Where(where).ExecuteAffrows();

            return res;
        }

        public async Task<int> DeleteWhereAsync(Expression<Func<T, bool>> where, CancellationToken? cancellationToken = default)
        {
            where.Should().NotBeNull();

            var res = await this.freeSql.Delete<T>().Where(where).ExecuteAffrowsAsync(cancellationToken: __token__(cancellationToken));

            return res;
        }

        public void Dispose()
        {
            //
        }

        public bool Exist(Expression<Func<T, bool>> where)
        {
            var selector = this.freeSql.Select<T>();
            if (where != null)
            {
                selector = selector.Where(where);
            }
            var res = selector.Any();
            return res;
        }

        public async Task<bool> ExistAsync(Expression<Func<T, bool>> where, CancellationToken? cancellationToken = default)
        {
            var selector = this.freeSql.Select<T>();
            if (where != null)
            {
                selector = selector.Where(where);
            }
            var res = await selector.AnyAsync(cancellationToken: __token__(cancellationToken));
            return res;
        }

        public int Insert(T model)
        {
            model.Should().NotBeNull();

            var res = this.freeSql.Insert<T>().AppendData(model).ExecuteAffrows();

            return res;
        }

        public async Task<int> InsertAsync(T model, CancellationToken? cancellationToken = default)
        {
            model.Should().NotBeNull();

            var res = await this.freeSql.Insert<T>().AppendData(model).ExecuteAffrowsAsync(cancellationToken: __token__(cancellationToken));

            return res;
        }

        public int QueryCount(Expression<Func<T, bool>> where)
        {
            var selector = this.freeSql.Select<T>();
            if (where != null)
            {
                selector = selector.Where(where);
            }
            var res = selector.Count();
            return (int)res;
        }

        public async Task<int> QueryCountAsync(Expression<Func<T, bool>> where, CancellationToken? cancellationToken = default)
        {
            var selector = this.freeSql.Select<T>();
            if (where != null)
            {
                selector = selector.Where(where);
            }
            var res = await selector.CountAsync(cancellationToken: __token__(cancellationToken));
            return (int)res;
        }

        protected virtual ISelect<T> __query_many__<OrderByColumnType>(
            Expression<Func<T, bool>> where,
            Expression<Func<T, OrderByColumnType>> orderby,
            bool desc,
            int? start,
            int? count)
        {
            var query = this.freeSql.Select<T>();
            if (where != null)
            {
                query = query.Where(where);
            }

            if (orderby != null)
            {
                if (desc)
                {
                    query = query.OrderByDescending(orderby);
                }
                else
                {
                    query = query.OrderBy(orderby);
                }
            }
            if (start != null)
            {
                orderby.Should().NotBeNull("使用skip前必须先排序");
                query = query.Skip(start.Value);
            }
            if (count != null)
            {
                query = query.Take(count.Value);
            }

            return query;
        }

        public T[] QueryMany<OrderByColumn>(Expression<Func<T, bool>> where, int? count = null, int? skip = null, Expression<Func<T, OrderByColumn>> order_by = null, bool desc = true)
        {
            var selector = this.__query_many__(where, order_by, desc, skip, count);

            var res = selector.ToList().ToArray();

            return res;
        }

        public async Task<T[]> QueryManyAsync<OrderByColumn>(Expression<Func<T, bool>> where,
            int? count = null, int? skip = null, Expression<Func<T, OrderByColumn>> order_by = null, bool desc = true,
            CancellationToken? cancellationToken = default)
        {
            var selector = this.__query_many__(where, order_by, desc, skip, count);

            var res = (await selector.ToListAsync(cancellationToken: __token__(cancellationToken))).ToArray();

            return res;
        }

        public T QueryOne(Expression<Func<T, bool>> where)
        {
            var data = this.QueryMany<object>(where, count: 1);

            var res = data.FirstOrDefault();

            return res;
        }

        public async Task<T> QueryOneAsync(Expression<Func<T, bool>> where, CancellationToken? cancellationToken = default)
        {
            var data = await this.QueryManyAsync<object>(where, count: 1, cancellationToken: cancellationToken);

            var res = data.FirstOrDefault();

            return res;
        }

        public int Update(T model)
        {
            model.Should().NotBeNull();

            var res = this.freeSql.Update<T>().SetSource(model).ExecuteAffrows();

            return res;
        }

        public async Task<int> UpdateAsync(T model, CancellationToken? cancellationToken = default)
        {
            model.Should().NotBeNull();

            var res = await this.freeSql.Update<T>().SetSource(model).ExecuteAffrowsAsync(cancellationToken: __token__(cancellationToken));

            return res;
        }

        public int InsertBulk(IEnumerable<T> models)
        {
            models.Should().NotBeNullOrEmpty();

            using var trans = this.freeSql.Ado.MasterPool.Get().Value.BeginTransaction();
            try
            {
                var res = this.freeSql.Insert<T>().AppendData(models).WithTransaction(trans).ExecuteAffrows();
                trans.Commit();
                return res;
            }
            catch
            {
                trans.Rollback();
                throw;
            }
        }

        public async Task<int> InsertBulkAsync(IEnumerable<T> models, CancellationToken? cancellationToken = default)
        {
            models.Should().NotBeNullOrEmpty();
            models.Any(x => string.IsNullOrWhiteSpace(x.Id)).Should().BeFalse();

            using var trans = this.freeSql.Ado.MasterPool.Get().Value.BeginTransaction();
            try
            {
                var res = await this.freeSql.Insert<T>().AppendData(models).WithTransaction(trans)
                    .ExecuteAffrowsAsync(cancellationToken: __token__(cancellationToken));

                trans.Commit();
                return res;
            }
            catch
            {
                trans.Rollback();
                throw;
            }
        }
    }
```