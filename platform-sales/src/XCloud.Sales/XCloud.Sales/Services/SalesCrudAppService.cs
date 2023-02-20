using System.Linq.Expressions;
using Volo.Abp.Application.Dtos;
using XCloud.Core.Dto;
using XCloud.Database.EntityFrameworkCore.Crud;
using XCloud.Sales.Core;
using XCloud.Sales.Data;

namespace XCloud.Sales.Services;

public interface ISalesPagingStringAppService<TEntity, TEntityDto, in TPagingRequest> :
    IXCloudPagingAppService<TEntity, TEntityDto, TPagingRequest, string>
    where TEntity : SalesBaseEntity<string>
    where TEntityDto : class, IEntityDto<string>
    where TPagingRequest : PagedRequest
{
    //
}

public abstract class SalesPagingStringAppService<TEntity, TEntityDto, TPagingRequest> :
    XCloudPagingAppService<TEntity, TEntityDto, TPagingRequest, string>,
    ISalesPagingStringAppService<TEntity, TEntityDto, TPagingRequest>
    where TEntity : SalesBaseEntity<string>
    where TEntityDto : class, IEntityDto<string>
    where TPagingRequest : PagedRequest
{
    protected SalesPagingStringAppService(ISalesRepository<TEntity> repository) : base(repository)
    {
        //
    }

    protected override Expression<Func<TEntity, bool>> BuildPrimaryKeyEqualExpression(string id)
    {
        return x => x.Id == id;
    }

    protected override bool IsEmptyKey(string id)
    {
        return string.IsNullOrWhiteSpace(id);
    }

    protected override async Task InitBeforeInsertAsync(TEntity entity)
    {
        await Task.CompletedTask;
        entity.Id = this.GuidGenerator.CreateGuidString();
    }

    protected override async Task<IOrderedQueryable<TEntity>> GetOrderedQueryableAsync(IQueryable<TEntity> query,
        TPagingRequest dto)
    {
        await Task.CompletedTask;
        return query.OrderByDescending(x => x.Id);
    }
}

//--------------------------------------------------

public interface ISalesPagingIntAppService<TEntity, TEntityDto, in TPagingRequest> :
    IXCloudPagingAppService<TEntity, TEntityDto, TPagingRequest, int>
    where TEntity : SalesBaseEntity
    where TEntityDto : class, IEntityDto<int>
    where TPagingRequest : PagedRequest
{
    //
}

public abstract class SalesPagingIntAppService<TEntity, TEntityDto, TPagingRequest> :
    XCloudPagingAppService<TEntity, TEntityDto, TPagingRequest, int>,
    ISalesPagingIntAppService<TEntity, TEntityDto, TPagingRequest>
    where TEntity : SalesBaseEntity
    where TEntityDto : class, IEntityDto<int>
    where TPagingRequest : PagedRequest
{
    protected SalesPagingIntAppService(ISalesRepository<TEntity> repository) : base(repository)
    {
        //
    }

    protected override Expression<Func<TEntity, bool>> BuildPrimaryKeyEqualExpression(int id)
    {
        return x => x.Id == id;
    }

    protected override bool IsEmptyKey(int id)
    {
        return id <= 0;
    }

    protected override async Task InitBeforeInsertAsync(TEntity entity)
    {
        await Task.CompletedTask;
        entity.Id = default;
    }

    protected override async Task<IOrderedQueryable<TEntity>> GetOrderedQueryableAsync(IQueryable<TEntity> query,
        TPagingRequest dto)
    {
        await Task.CompletedTask;
        return query.OrderByDescending(x => x.Id);
    }
}