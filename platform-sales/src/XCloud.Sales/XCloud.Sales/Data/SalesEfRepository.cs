using XCloud.Database.EntityFrameworkCore.Repository;
using XCloud.Sales.Core;
using XCloud.Sales.Data.Database;

namespace XCloud.Sales.Data;

/// <summary>
/// Represents an entity repository
/// </summary>
/// <typeparam name="TEntity">Entity type</typeparam>
public interface ISalesRepository<TEntity> : IEfRepository<TEntity>, IScopedDependency where TEntity : class, ISalesBaseEntity
{
    //
}
    
/// <summary>
/// Represents the Entity Framework repository
/// </summary>
/// <typeparam name="TEntity">Entity type</typeparam>
public class SalesEfRepository<TEntity> : EfRepositoryBase<ShopDbContext, TEntity>, ISalesRepository<TEntity> where TEntity : class, ISalesBaseEntity
{
    public SalesEfRepository(IServiceProvider serviceProvider) : base(serviceProvider)
    {
        //
    }
}