using System;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp.Auditing;
using Volo.Abp.Domain.Entities;
using Volo.Abp.Domain.Repositories;
using XCloud.Core.Application;
using XCloud.Core.Dto;
using XCloud.Core.Extension;
using XCloud.Core.Helper;
using XCloud.Database.EntityFrameworkCore.Crud.Helper;
using XCloud.Database.EntityFrameworkCore.Repository;

namespace XCloud.Database.EntityFrameworkCore.Extensions;

public static class RepositoryExtension
{
    /// <summary>
    /// check input=>delete by uids
    /// </summary>
    public static async Task DeleteByIdsAsync<T>(this IEfRepository<T> repo, string[] uids) where T : EntityBase
    {
        if (uids == null)
            throw new ArgumentNullException(nameof(uids));
        if(!uids.Any())
            return;

        await repo.DeleteAsync(x => uids.Contains(x.Id));
    }

    public static async Task<ApiResponse<T>> AddTreeNode<T>(this IEfRepository<T> repo, T model) where T : TreeEntityBase
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

    public static async Task DeleteTreeNodeRecursively<T>(this IEfRepository<T> repo, string node_uid) where T : TreeEntityBase
    {
        var node = await repo.QueryOneAsync(x => x.Id == node_uid);
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

    public static async Task<bool> DeleteSingleNodeWhenNoChildren<T>(this IEfRepository<T> repo, string node_uid) where T : TreeEntityBase
    {
        if (await repo.AnyAsync(x => x.ParentId == node_uid))
        {
            return false;
        }

        await repo.DeleteAsync(x => x.Id == node_uid);

        return true;
    }

    public static async Task SoftDeleteByIdAsync<T>(this IEfRepository<T> repo, string[] uids) where T : class, IEntityBase, IHasDeletionTime
    {
        if (ValidateHelper.IsEmptyCollection(uids))
            throw new ArgumentNullException(nameof(uids));

        var db = await repo.GetDbContextAsync();

        await db.UpdateEntity<T>().IgnoreFilter()
            .Where(x => x.IsDeleted == false)
            .Where(x => uids.Contains(x.Id))
            .SetIsDeleted(true)
            .ExecuteAsync();
    }

    public static async Task RevokeSoftDeletionByIdAsync<T>(this IEfRepository<T> repo, string[] uids) where T : class, IEntityBase, IHasDeletionTime
    {
        if (ValidateHelper.IsEmptyCollection(uids))
            throw new ArgumentNullException(nameof(uids));

        var db = await repo.GetDbContextAsync();

        await db.UpdateEntity<T>().IgnoreFilter()
            .Where(x => x.IsDeleted == true)
            .Where(x => uids.Contains(x.Id))
            .SetIsDeleted(false)
            .ExecuteAsync();
    }
}