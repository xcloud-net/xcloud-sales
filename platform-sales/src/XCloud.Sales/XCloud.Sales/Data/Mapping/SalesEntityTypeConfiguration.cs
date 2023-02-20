using XCloud.Database.EntityFrameworkCore.Mapping;
using XCloud.Sales.Core;

namespace XCloud.Sales.Data.Mapping;

/// <summary>
/// Represents base entity mapping configuration
/// </summary>
/// <typeparam name="TEntity">Entity type</typeparam>
public abstract class SalesEntityTypeConfiguration<TEntity> : EfEntityTypeConfiguration<TEntity> where TEntity : class, ISalesBaseEntity
{
    //
}