using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.Domain.Entities;
using XCloud.Application.Service;
using XCloud.Core.Cache;
using XCloud.Core.Dto;
using XCloud.Database.EntityFrameworkCore.Extensions;
using XCloud.Platform.Core.Application;
using XCloud.Platform.Core.Domain.Notification;
using XCloud.Platform.Data.Database;

namespace XCloud.Platform.Application.Common.Service.Notification;

public interface INotificationService : IXCloudApplicationService
{
    Task<SysNotificationDto> QueryByIdAsync(string notificationId);

    Task<SysNotification> InsertAsync(SysNotificationDto dto);

    Task UpdateStatusAsync(UpdateNotificationStatusInput dto);

    Task DeleteAsync(string notificationId);

    Task<PagedResponse<SysNotificationDto>> QueryPagingAsync(QueryNotificationInput dto);

    Task<int> UnreadCountAsync(string userId);

    Task<int> UnreadCountAsync(string userId, CachePolicy cachePolicyOption);
}

public class NotificationService : PlatformApplicationService, INotificationService
{
    private readonly IPlatformRepository<SysNotification> _repository;

    public NotificationService(IPlatformRepository<SysNotification> repository)
    {
        this._repository = repository;
    }

    public async Task<SysNotificationDto> QueryByIdAsync(string notificationId)
    {
        var entity = await this._repository.QueryOneAsync(x => x.Id == notificationId);

        if (entity == null)
            return null;

        var dto = this.ObjectMapper.Map<SysNotification, SysNotificationDto>(entity);

        return dto;
    }

    public async Task<SysNotification> InsertAsync(SysNotificationDto dto)
    {
        var entity = this.ObjectMapper.Map<SysNotificationDto, SysNotification>(dto);

        entity.Id = this.GuidGenerator.CreateGuidString();
        entity.CreationTime = this.Clock.Now;
        entity.LastModificationTime = entity.CreationTime;

        await this._repository.InsertAsync(entity);

        return entity;
    }

    public async Task UpdateStatusAsync(UpdateNotificationStatusInput dto)
    {
        var db = await this._repository.GetDbContextAsync();
        var set = db.Set<SysNotification>();

        var entity = await set.FirstOrDefaultAsync(x => x.Id == dto.Id);

        if (entity == null)
            throw new EntityNotFoundException(nameof(UpdateStatusAsync));

        if (dto.Read != null)
        {
            if (!entity.Read && entity.ReadTime == null && dto.Read.Value)
                entity.ReadTime = this.Clock.Now;

            entity.Read = dto.Read.Value;
        }

        entity.LastModificationTime = this.Clock.Now;

        await db.TrySaveChangesAsync();
    }

    public async Task DeleteAsync(string notificationId)
    {
        var entity = await this._repository.QueryOneAsync(x => x.Id == notificationId);

        if (entity == null)
            throw new ArgumentNullException(nameof(DeleteAsync));

        await this._repository.DeleteAsync(entity);
        await this.UnreadCountAsync(entity.UserId, new CachePolicy() { Refresh = true });
    }

    public async Task<PagedResponse<SysNotificationDto>> QueryPagingAsync(QueryNotificationInput dto)
    {
        if (string.IsNullOrWhiteSpace(dto.UserId))
            throw new ArgumentNullException(nameof(dto.UserId));

        var db = await this._repository.GetDbContextAsync();

        var query = from notice in db.Set<SysNotification>().AsNoTracking()
            select new { notice };

        query = query.Where(x => x.notice.UserId == dto.UserId);

        if (!string.IsNullOrWhiteSpace(dto.AppKey))
            query = query.Where(x => x.notice.App == dto.AppKey);

        var count = 0;
        if (!dto.SkipCalculateTotalCount)
            count = await query.CountAsync();

        var data = await query.PageBy(dto.ToAbpPagedRequest())
            .OrderByDescending(x => x.notice.CreationTime)
            .ToArrayAsync();

        var items = new List<SysNotificationDto>();

        foreach (var m in data)
        {
            var item = this.ObjectMapper.Map<SysNotification, SysNotificationDto>(m.notice);
            items.Add(item);
        }

        return new PagedResponse<SysNotificationDto>(items, dto, count);
    }

    public async Task<int> UnreadCountAsync(string userId, CachePolicy cachePolicyOption)
    {
        var key = $"platform.user.{userId}.notification.count";
        var option = new CacheOption<int>(key, TimeSpan.FromMinutes(30));
        var count = await this.CacheProvider.ExecuteWithPolicyAsync(() => this.UnreadCountAsync(userId), option,
            cachePolicyOption);
        return count;
    }

    public async Task<int> UnreadCountAsync(string userId)
    {
        if (string.IsNullOrWhiteSpace(userId))
            throw new ArgumentNullException(nameof(UnreadCountAsync));

        var db = await this._repository.GetDbContextAsync();

        var query = from notice in db.Set<SysNotification>().AsNoTracking()
            select new { notice };

        var count = await query
            .Where(x => x.notice.UserId == userId)
            .Where(x => !x.notice.Read)
            .CountAsync();

        return count;
    }
}