using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Volo.Abp;
using Volo.Abp.Auditing;
using Volo.Abp.Domain.Entities;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Timing;
using XCloud.Core.Application.Entity;
using XCloud.Core.Dto;
using XCloud.Core.Extension;
using XCloud.Core.Helper;
using XCloud.Database.EntityFrameworkCore.Repository;

namespace XCloud.Database.EntityFrameworkCore.Extensions;

public static class RepositoryExtension
{
    public static async Task<ApiResponse<T>> AddTreeNode<T>(this IEfRepository<T> repo, T model)
        where T : TreeEntityBase
    {
        if (string.IsNullOrWhiteSpace(model?.Id))
            throw new ArgumentNullException(nameof(model));

        var res = new ApiResponse<T>();

        if (model.IsFirstLevel())
        {
            model.AsFirstLevel();
        }
        else
        {
            var parent = await repo.QueryOneAsync(x => x.Id == model.ParentId);
            if (parent == null)
                throw new EntityNotFoundException("父节点为空");
        }

        if (!model.IsValid(out var msg))
        {
            res.SetError(msg);
            return res;
        }

        await repo.InsertAsync(model);
        return res.SetData(model);
    }

    public static async Task DeleteTreeNodeRecursively<T>(this IEfRepository<T> repo, string nodeId)
        where T : TreeEntityBase
    {
        var node = await repo.QueryOneAsync(x => x.Id == nodeId);
        if (node == null)
            throw new EntityNotFoundException(nameof(node));

        var list = await repo.QueryManyAsync(x => x.TreeGroupKey == node.TreeGroupKey);
        if (list.Length > 5000)
            throw new NotSupportedException("too many nodes to delete");

        var toDeleted = list.FindNodeChildrenRecursively(node).Select(x => x.Id).ToArray();

        if (toDeleted.Any())
        {
            await repo.DeleteAsync(x => toDeleted.Contains(x.Id));
        }
    }

    public static async Task<bool> DeleteSingleNodeWhenNoChildren<T>(this IEfRepository<T> repo, string nodeId)
        where T : TreeEntityBase
    {
        if (await repo.AnyAsync(x => x.ParentId == nodeId))
        {
            return false;
        }

        await repo.DeleteAsync(x => x.Id == nodeId);

        return true;
    }

    public static async Task SoftDeleteByIdAsync<T>(this IEfRepository<T> repo, string[] ids, IClock clock)
        where T : class, IEntityBase, ISoftDelete
    {
        if (ValidateHelper.IsEmptyCollection(ids))
            throw new ArgumentNullException(nameof(ids));

        if (clock == null)
            throw new ArgumentNullException(nameof(clock));

        var db = await repo.GetDbContextAsync();

        var entity = await db.Set<T>().FirstOrDefaultAsync(x => ids.Contains(x.Id));

        if (entity == null)
            throw new EntityNotFoundException(nameof(entity));

        if (entity is ISoftDelete softDelete)
            softDelete.IsDeleted = true;
        else
            throw new NotSupportedException("entity is not soft delete entity");

        if (entity is IHasDeletionTime hasDeletionTime)
            hasDeletionTime.DeletionTime = clock.Now;

        await repo.UpdateAsync(entity);
    }

    public static async Task RevokeSoftDeletionByIdAsync<T>(this IEfRepository<T> repo, string[] ids)
        where T : class, IEntityBase, ISoftDelete
    {
        if (ValidateHelper.IsEmptyCollection(ids))
            throw new ArgumentNullException(nameof(ids));

        var db = await repo.GetDbContextAsync();

        var entity = await db.Set<T>().IgnoreQueryFilters()
            .FirstOrDefaultAsync(x => ids.Contains(x.Id));

        if (entity == null)
            throw new EntityNotFoundException(nameof(entity));

        if (entity is ISoftDelete softDelete)
            softDelete.IsDeleted = false;
        else
            throw new NotSupportedException("entity is not soft delete entity");

        if (entity is IHasDeletionTime hasDeletionTime)
            hasDeletionTime.DeletionTime = null;

        await repo.UpdateAsync(entity);
    }
}