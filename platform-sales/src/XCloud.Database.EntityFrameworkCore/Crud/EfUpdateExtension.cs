using System;
using Microsoft.EntityFrameworkCore;
using Volo.Abp.Auditing;
using XCloud.Core.Application;
using XCloud.Core.Application.Entity;

namespace XCloud.Database.EntityFrameworkCore.Crud;

public static class EfUpdateExtension
{
    /// <summary>
    /// 更新实体
    /// </summary>
    public static IEfUpdate<T> UpdateEntity<T>(this DbContext db, int? maxBatchSize = null) where T : class, IEntityBase
    {
        var res = new EfUpdate<T>(db, maxBatchSize);
        return res;
    }

    public static IEfUpdate<T> SetUpdateTime<T>(this IEfUpdate<T> updator, DateTime time) where T : class, IEntityBase, IHasModificationTime
    {
        updator = updator.SetField(x => x.LastModificationTime, time);
        return updator;
    }

    public static IEfUpdate<T> SetIsDeleted<T>(this IEfUpdate<T> updator, bool isDeleted) where T : class, IEntityBase, IHasDeletionTime
    {
        updator = updator.SetEntity(x =>
        {
            x.IsDeleted = isDeleted;
            x.DeletionTime = null;
            if (isDeleted)
            {
                x.DeletionTime = DateTime.UtcNow;
            }
        });
        return updator;
    }
}