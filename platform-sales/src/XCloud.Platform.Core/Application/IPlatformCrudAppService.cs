using System.Linq.Expressions;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using XCloud.Core.Application;
using XCloud.Database.EntityFrameworkCore.Crud;
using XCloud.Database.EntityFrameworkCore.Repository;

namespace XCloud.Platform.Core.Application;

public interface
    IPlatformCrudAppService<TEntity, in TEntityDto> : IXCloudCrudAppService<TEntity, TEntityDto, string>
    where TEntity : EntityBase
    where TEntityDto : class, IEntityDto<string>
{
    //
}

public abstract class PlatformCrudAppService<TEntity, TEntityDto> :
    XCloudCrudAppService<TEntity, TEntityDto, string>,
    IPlatformCrudAppService<TEntity, TEntityDto>
    where TEntity : EntityBase
    where TEntityDto : class, IEntityDto<string>
{
    protected PlatformCrudAppService(IEfRepository<TEntity> repository) : base(repository)
    {
        //
    }

    protected override Expression<Func<TEntity, bool>> BuildPrimaryKeyEqualExpression(string id)
    {
        return x => x.Id == id;
    }

    protected override void TrySetCreationTime(TEntity entity)
    {
        entity.CreationTime = this.Clock.Now;
    }

    protected override void TrySetPrimaryKey(TEntity entity)
    {
        entity.Id = this.GuidGenerator.CreateGuidString();
    }

    protected override bool IsEmptyKey(string id)
    {
        return string.IsNullOrWhiteSpace(id);
    }

    protected override async Task InitBeforeInsertAsync(TEntity entity)
    {
        await Task.CompletedTask;
        
        entity.Id = this.GuidGenerator.CreateGuidString();
        entity.CreationTime = this.Clock.Now;
    }
}