using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Volo.Abp;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Auditing;
using Volo.Abp.Domain.Entities;
using XCloud.Application.Mapper;
using XCloud.Core.Dto;
using XCloud.Database.EntityFrameworkCore.Repository;

namespace XCloud.Application.Service;

public interface
    IXCloudPagingAppService<TEntity, TEntityDto, in TPagingRequest, in TKey> : IXCloudCrudAppService<
        TEntity,
        TEntityDto, TKey>
    where TEntity : class, IEntity<TKey>
    where TEntityDto : class, IEntityDto<TKey>
    where TPagingRequest : PagedRequest
{
    Task<PagedResponse<TEntityDto>> QueryPagingAsync(TPagingRequest dto);
}

public abstract class
    XCloudPagingAppService<TEntity, TEntityDto, TPagingRequest, TKey> :
        XCloudCrudAppService<TEntity, TEntityDto, TKey>,
        IXCloudPagingAppService<TEntity, TEntityDto, TPagingRequest, TKey>
    where TEntity : class, IEntity<TKey>
    where TEntityDto : class, IEntityDto<TKey>
    where TPagingRequest : PagedRequest
{
    protected XCloudPagingAppService(IEfRepository<TEntity> repository) : base(repository)
    {
        //
    }

    protected virtual async Task<IOrderedQueryable<TEntity>> GetPagingOrderedQueryableAsync(IQueryable<TEntity> query,
        TPagingRequest dto)
    {
        this.Logger.LogWarning($"please override {nameof(GetPagingOrderedQueryableAsync)} to speed up your code");

        await Task.CompletedTask;

        if (typeof(TEntity).IsAssignableTo<IHasCreationTime>())
        {
            var entityType = typeof(TEntity);
            //x
            var parameterExpr = Expression.Parameter(entityType, "x");

            var creationTimeProperty = entityType.GetProperties()
                .FirstOrDefault(x => x.Name == nameof(IHasCreationTime.CreationTime));

            if (creationTimeProperty == null)
                throw new AbpException(nameof(creationTimeProperty));

            if (creationTimeProperty.PropertyType != typeof(DateTime))
                throw new AbpException("wrong creation time type");

            //x.Id
            var fieldSelectorExpr =
                Expression.Property(expression: parameterExpr, property: creationTimeProperty);

            var expression =
                Expression.Lambda<Func<TEntity, DateTime>>(fieldSelectorExpr,
                    new ParameterExpression[] { parameterExpr });

            return query.OrderByDescending(expression);
        }

        return query.OrderByDescending(x => x.Id);
    }

    protected virtual async Task<IQueryable<TEntity>> GetPagingFilteredQueryableAsync(IQueryable<TEntity> query,
        TPagingRequest dto)
    {
        await Task.CompletedTask;
        return query;
    }

    protected virtual async Task<IQueryable<TEntity>> GetPagingQueryableAsync(DbContext db, TPagingRequest dto)
    {
        await Task.CompletedTask;
        return db.Set<TEntity>().AsNoTracking();
    }

    public virtual async Task<PagedResponse<TEntityDto>> QueryPagingAsync(TPagingRequest dto)
    {
        if (dto == null)
            throw new ArgumentNullException(nameof(dto));

        var db = await this.Repository.GetDbContextAsync();

        var query = await this.GetPagingQueryableAsync(db, dto);

        query = await this.GetPagingFilteredQueryableAsync(query, dto);

        var count = 0;
        if (!dto.SkipCalculateTotalCount)
        {
            count = await query.CountAsync();
        }

        var orderedQuery = await this.GetPagingOrderedQueryableAsync(query, dto);
        
        var datalist = await orderedQuery.PageBy(dto.ToAbpPagedRequest()).ToArrayAsync();

        var items = this.ObjectMapper.MapArray<TEntity, TEntityDto>(datalist);

        return new PagedResponse<TEntityDto>(items, dto, count);
    }
}