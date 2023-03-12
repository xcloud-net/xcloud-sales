using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using XCloud.Application.Service;
using XCloud.Core.Application;
using XCloud.Core.Application.Entity;
using XCloud.Core.Dto;
using XCloud.Database.EntityFrameworkCore.Repository;

namespace XCloud.Platform.Core.Application;

public interface
    IPlatformPagingCrudService<TEntity, TEntityDto, in TPagingRequest> : IXCloudPagingAppService<
        TEntity,
        TEntityDto, TPagingRequest, string>
    where TEntity : EntityBase
    where TEntityDto : class, IEntityDto<string>
    where TPagingRequest : PagedRequest
{
    //
}

public abstract class
    PlatformPagingCrudService<TEntity, TEntityDto, TPagingRequest> :
        XCloudPagingAppService<TEntity, TEntityDto, TPagingRequest, string>,
        IPlatformPagingCrudService<TEntity, TEntityDto, TPagingRequest>
    where TEntity : EntityBase
    where TEntityDto : class, IEntityDto<string>
    where TPagingRequest : PagedRequest
{
    protected PlatformPagingCrudService(IEfRepository<TEntity> repository) : base(repository)
    {
        //
    }

    protected override async Task<IOrderedQueryable<TEntity>> GetPagingOrderedQueryableAsync(IQueryable<TEntity> query,
        TPagingRequest dto)
    {
        await Task.CompletedTask;
        return query.OrderByDescending(x => x.CreationTime);
    }
}