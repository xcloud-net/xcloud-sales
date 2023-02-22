using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Auditing;
using Volo.Abp.Domain.Entities;
using XCloud.Core.Application;
using XCloud.Core.Helper;
using XCloud.Core.IdGenerator;
using XCloud.Database.EntityFrameworkCore.Repository;

namespace XCloud.Application.Service;

public interface IXCloudCrudAppService<TEntity, in TEntityDto, in TKey> : IXCloudApplicationService
    where TEntity : class, IEntity<TKey>
    where TEntityDto : class, IEntityDto<TKey>
{
    Task<int> QueryCountAsync();

    Task<TEntity> QueryByIdAsync(TKey id);

    Task<TEntity[]> QueryByIdsAsync(TKey[] ids);

    Task<TEntity> InsertAsync(TEntityDto dto);

    Task<TEntity> UpdateAsync(TEntityDto dto);

    Task DeleteByIdAsync(TKey id);

    Task DeleteByIdsAsync(TKey[] ids);
}

/// <summary>
/// 底层实现，使用了反射，性能不是最佳。
/// 当遇到具体业务场景建议override函数来提供更高性能实现
/// </summary>
public abstract class XCloudCrudAppService<TEntity, TEntityDto, TKey> :
    XCloudApplicationService,
    IXCloudCrudAppService<TEntity, TEntityDto, TKey>
    where TEntity : class, IEntity<TKey>
    where TEntityDto : class, IEntityDto<TKey>
{
    protected IEfRepository<TEntity> Repository { get; private set; }

    protected XCloudCrudAppService(IEfRepository<TEntity> repository)
    {
        this.Repository = repository;
    }

    public virtual async Task<int> QueryCountAsync()
    {
        var query = await this.Repository.GetQueryableAsync();

        return await query.CountAsync();
    }

    protected virtual bool IsEmptyKey(TKey id)
    {
        this.Logger.LogWarning($"please override {nameof(IsEmptyKey)} to speed up your code");

        var keyType = typeof(TKey);

        if (keyType == typeof(string))
        {
            var notEmpty = id is string strId && !string.IsNullOrWhiteSpace(strId);
            return !notEmpty;
        }

        if (keyType == typeof(int))
        {
            var notEmpty = id is int intId && intId > 0;
            return !notEmpty;
        }

        if (keyType == typeof(long))
        {
            var notEmpty = id is long longId && longId > 0;
            return !notEmpty;
        }

        throw new NotSupportedException(nameof(IsEmptyKey));
    }

    protected virtual async Task CheckBeforeInsertAsync(TEntityDto dto)
    {
        await Task.CompletedTask;
    }

    protected virtual void TrySetPrimaryKey(TEntity entity)
    {
        var properties = typeof(TEntity).GetProperties();

        var primaryProperty = properties.FirstOrDefault(x => x.Name == nameof(IEntity<TKey>.Id) && x.CanWrite);

        if (primaryProperty != null)
        {
            if (primaryProperty.PropertyType == typeof(string))
            {
                primaryProperty.SetValue(entity, this.GuidGenerator.CreateGuidString());
            }
            else if (primaryProperty.PropertyType == typeof(int))
            {
                primaryProperty.SetValue(entity, default(int));
            }
            else if (primaryProperty.PropertyType == typeof(long))
            {
                primaryProperty.SetValue(entity, default(long));
            }
            else
            {
                throw new NotSupportedException(nameof(primaryProperty));
            }
        }
    }

    protected virtual void TrySetCreationTime(TEntity entity)
    {
        var properties = typeof(TEntity).GetProperties();

        var creationTimeProperty =
            properties.FirstOrDefault(x =>
                x.Name == nameof(IHasCreationTime.CreationTime) &&
                x.CanWrite &&
                x.PropertyType == typeof(DateTime));

        if (creationTimeProperty != null)
        {
            creationTimeProperty.SetValue(obj: entity, this.Clock.Now);
        }
    }

    protected virtual async Task InitBeforeInsertAsync(TEntity entity)
    {
        this.Logger.LogWarning($"please override {nameof(InitBeforeInsertAsync)} to speed up your code");

        await Task.CompletedTask;

        this.TrySetPrimaryKey(entity);
        this.TrySetCreationTime(entity);
    }

    protected virtual async Task CheckBeforeUpdateAsync(TEntityDto dto)
    {
        await Task.CompletedTask;
    }

    protected virtual Expression<Func<TEntity, bool>> BuildPrimaryKeyEqualExpression(TKey id)
    {
        Expression<Func<TEntity, bool>> expression = x => x.Id.Equals(id);

        return expression;
    }

    public virtual async Task<TEntity> QueryByIdAsync(TKey id)
    {
        if (IsEmptyKey(id))
            throw new ArgumentNullException(nameof(id));

        var entity = await this.Repository.QueryOneAsync(this.BuildPrimaryKeyEqualExpression(id));

        return entity;
    }

    public virtual async Task<TEntity[]> QueryByIdsAsync(TKey[] ids)
    {
        if (ValidateHelper.IsEmptyCollection(ids))
            throw new ArgumentNullException(nameof(ids));

        var db = await this.Repository.GetDbContextAsync();

        var entities = await db.Set<TEntity>().AsNoTracking().Where(x => ids.Contains(x.Id)).ToArrayAsync();

        return entities;
    }

    public virtual async Task DeleteByIdAsync(TKey id)
    {
        if (IsEmptyKey(id))
            throw new ArgumentNullException(nameof(id));

        await this.Repository.DeleteAsync(this.BuildPrimaryKeyEqualExpression(id));
    }

    public virtual async Task DeleteByIdsAsync(TKey[] ids)
    {
        if (ValidateHelper.IsEmptyCollection(ids))
            throw new ArgumentNullException(nameof(ids));

        await this.Repository.DeleteAsync(x => ids.Contains(x.Id));
    }

    protected virtual async Task ModifyFieldsForUpdateAsync(TEntity entity, TEntityDto dto)
    {
        this.Logger.LogWarning($"you are using default entity fields modification method");

        await Task.CompletedTask;

        var ignoreProperty = new[]
        {
            nameof(IEntity<TKey>.Id),
            nameof(IHasCreationTime.CreationTime)
        };

        entity.SetEntityFields(dto, x => ignoreProperty.Contains(x.Name));

        if (entity is IHasModificationTime modificationTime)
        {
            modificationTime.LastModificationTime = this.Clock.Now;
        }
    }

    public virtual async Task<TEntity> InsertAsync(TEntityDto dto)
    {
        if (dto == null)
            throw new ArgumentNullException(nameof(dto));

        if (!IsEmptyKey(dto.Id))
            throw new ArgumentException($"{nameof(dto.Id)} should be empty");

        await this.CheckBeforeInsertAsync(dto);

        var entity = this.ObjectMapper.Map<TEntityDto, TEntity>(dto);

        await this.InitBeforeInsertAsync(entity);

        await this.Repository.InsertAsync(entity);

        return entity;
    }

    public virtual async Task<TEntity> UpdateAsync(TEntityDto dto)
    {
        if (dto == null)
            throw new ArgumentNullException(nameof(dto));

        if (IsEmptyKey(dto.Id))
            throw new ArgumentNullException(nameof(dto.Id));

        var entity = await this.Repository.QueryOneAsync(this.BuildPrimaryKeyEqualExpression(dto.Id));

        if (entity == null)
            throw new EntityNotFoundException(nameof(entity));

        await this.CheckBeforeUpdateAsync(dto);

        await this.ModifyFieldsForUpdateAsync(entity, dto);

        await this.Repository.UpdateAsync(entity);

        return entity;
    }
}