using DotNetCore.CAP;
using Volo.Abp.Application.Services;
using XCloud.Core.Dto;
using XCloud.Sales.Data.Domain.Logging;
using XCloud.Sales.Data.Domain.Orders;
using XCloud.Sales.Services.Catalog;
using XCloud.Sales.Services.Logging;
using XCloud.Sales.Services.ShoppingCart;

namespace XCloud.Sales.Queue;

/// <summary>
/// Redis缓存接口
/// </summary>
public interface IEventBusService : IApplicationService
{
    Task FormatGoodsSpecCombinationAsync(FormatGoodsSpecCombinationInput dto);
    Task NotifySyncUserInfoFromPlatformAsync(int userId);
    Task NotifyRefreshUserInfoAsync(int userId);
    Task NotifySetUserLastActivityTimeAsync(int userId);
    Task NotifyRefreshGoodsInfo(IdDto<int> dto);
    Task NotifyInsertOrderNote(OrderNote orderNote);
    Task NotifyClearActivityLog();
    Task NotifyRemoveCartsBySpecs(RemoveCartBySpecs dto);
    Task NotifyInsertActivityLog(ActivityLog log);
}

public class EventBusService : SalesAppService, IEventBusService
{
    private readonly ICapPublisher _capPublisher;

    public EventBusService(ICapPublisher capPublisher)
    {
        this._capPublisher = capPublisher;
    }

    public async Task FormatGoodsSpecCombinationAsync(FormatGoodsSpecCombinationInput dto)
    {
        if (dto == null || dto.Id <= 0)
            throw new ArgumentNullException(nameof(dto));

        await this._capPublisher.PublishAsync(SalesMessageTopics.FormatCombinationSpecs, dto);
    }

    public async Task NotifySyncUserInfoFromPlatformAsync(int userId)
    {
        if (userId <= 0)
            return;
        await this._capPublisher.PublishAsync(SalesMessageTopics.SyncUserInfoFromPlatform, new IdDto<int>(userId));
    }

    public async Task NotifyRefreshUserInfoAsync(int userId)
    {
        if (userId <= 0)
            return;
        await this._capPublisher.PublishAsync(SalesMessageTopics.RefreshUserInformation, new IdDto<int>(userId));
    }

    public async Task NotifySetUserLastActivityTimeAsync(int userId)
    {
        if (userId <= 0)
            return;
        await this._capPublisher.PublishAsync(SalesMessageTopics.UpdateUserLastActivityTime, new IdDto<int>(userId));
    }

    public async Task NotifyRefreshGoodsInfo(IdDto<int> dto)
    {
        await this._capPublisher.PublishAsync(SalesMessageTopics.RefreshGoodsInfo, dto);
    }

    public async Task NotifyInsertOrderNote(OrderNote orderNote)
    {
        if (orderNote == null)
            throw new ArgumentNullException(nameof(orderNote));

        await this._capPublisher.PublishAsync(SalesMessageTopics.InsertOrderNote, orderNote);
    }

    public async Task NotifyClearActivityLog()
    {
        await this._capPublisher.PublishAsync(SalesMessageTopics.ClearAllActivityLogs, string.Empty);
    }

    public async Task NotifyInsertActivityLog(ActivityLog log)
    {
        if (log == null)
            throw new ArgumentNullException(nameof(log));

        var activityLogService = this.LazyServiceProvider.LazyGetRequiredService<IActivityLogService>();

        log = activityLogService.AttachHttpContextInfo(log);

        await this._capPublisher.PublishAsync(SalesMessageTopics.InsertActivityLog, log);
    }

    public async Task NotifyRemoveCartsBySpecs(RemoveCartBySpecs dto)
    {
        if (dto == null)
            throw new ArgumentNullException(nameof(dto));

        await this._capPublisher.PublishAsync(SalesMessageTopics.RemoveCartsAfterPlaceOrder, dto);
    }
}