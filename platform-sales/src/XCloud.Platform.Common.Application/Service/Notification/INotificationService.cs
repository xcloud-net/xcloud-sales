using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.Domain.Entities;
using XCloud.Application.Service;
using XCloud.Core.Application;
using XCloud.Core.Cache;
using XCloud.Core.Dto;
using XCloud.Database.EntityFrameworkCore.Extensions;
using XCloud.Platform.Core.Application;
using XCloud.Platform.Core.Domain.Notification;
using XCloud.Platform.Data.Database;

namespace XCloud.Platform.Common.Application.Service.Notification;

public interface INotificationService : IXCloudApplicationService
{
    Task<SysNotificationDto> QueryByIdAsync(string notificationId);

    Task<SysNotification> InsertNotificationAsync(SysNotificationDto dto);

    Task UpdateNotificationStatusAsync(UpdateNotificationStatusInput dto);

    Task DeleteAsync(string notificationId);

    Task<PagedResponse<SysNotificationDto>> QueryPaginationAsync(QueryNotificationInput dto);

    Task<int> UnreadCountAsync(string userId);

    Task<int> UnreadCountAsync(string userId, CachePolicy cachePolicyOption);
}

public class NotificationService : PlatformApplicationService, INotificationService
{
    private readonly IPlatformRepository<SysNotification> _repo;

    public NotificationService(IPlatformRepository<SysNotification> repo)
    {
        this._repo = repo;
    }

    public async Task<SysNotificationDto> QueryByIdAsync(string notificationId)
    {
        var entity = await this._repo.QueryOneAsync(x => x.Id == notificationId);

        if (entity == null)
            return null;

        var dto = this.ObjectMapper.Map<SysNotification, SysNotificationDto>(entity);

        return dto;
    }

    public async Task<SysNotification> InsertNotificationAsync(SysNotificationDto dto)
    {
        var entity = this.ObjectMapper.Map<SysNotificationDto, SysNotification>(dto);

        entity.Id = this.GuidGenerator.CreateGuidString();
        entity.CreationTime = this.Clock.Now;

        await this._repo.InsertAsync(entity);

        return entity;
    }

    public async Task UpdateNotificationStatusAsync(UpdateNotificationStatusInput dto)
    {
        var db = await this._repo.GetDbContextAsync();
        var set = db.Set<SysNotification>();

        var entity = await set.FirstOrDefaultAsync(x => x.Id == dto.Id);

        if (entity == null)
            throw new EntityNotFoundException(nameof(UpdateNotificationStatusAsync));

        if (dto.Read != null)
        {
            if (!entity.Read && entity.ReadTime == null && dto.Read.Value)
                entity.ReadTime = this.Clock.Now;

            entity.Read = dto.Read.Value;
        }

        await db.TrySaveChangesAsync();
    }

    public async Task DeleteAsync(string notificationId)
    {
        var entity = await this._repo.QueryOneAsync(x => x.Id == notificationId);

        if (entity == null)
            throw new ArgumentNullException(nameof(DeleteAsync));

        await this._repo.DeleteAsync(entity);
        await this.UnreadCountAsync(entity.UserId, new CachePolicy() { Refresh = true });
    }

    public async Task<PagedResponse<SysNotificationDto>> QueryPaginationAsync(QueryNotificationInput dto)
    {
        if (string.IsNullOrWhiteSpace(dto.UserId))
            throw new ArgumentNullException(nameof(dto.UserId));

        var db = await this._repo.GetDbContextAsync();

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

        var db = await this._repo.GetDbContextAsync();

        var query = from notice in db.Set<SysNotification>().AsNoTracking()
            select new { notice };

        var count = await query
            .Where(x => x.notice.UserId == userId)
            .Where(x => !x.notice.Read)
            .CountAsync();

        return count;
    }
}