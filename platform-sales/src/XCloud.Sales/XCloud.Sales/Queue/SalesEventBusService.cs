using DotNetCore.CAP;
using XCloud.Core.Dto;
using XCloud.Sales.Application;
using XCloud.Sales.Data.Domain.Logging;
using XCloud.Sales.Data.Domain.Orders;
using XCloud.Sales.Service.Catalog;
using XCloud.Sales.Service.Logging;
using XCloud.Sales.Service.Orders;
using XCloud.Sales.Service.ShoppingCart;

namespace XCloud.Sales.Queue;

public class SalesEventBusService : SalesAppService
{
    private readonly ICapPublisher _capPublisher;

    public SalesEventBusService(ICapPublisher capPublisher)
    {
        this._capPublisher = capPublisher;
    }

    public virtual async Task OrderCreatedAsync(OrderDto dto)
    {
        await this._capPublisher.PublishAsync(SalesMessageTopics.OrderCreated, dto);
    }

    public virtual async Task OrderShippedAsync(OrderDto dto)
    {
        await this._capPublisher.PublishAsync(SalesMessageTopics.OrderShipped, dto);
    }

    public virtual async Task OrderCanceledAsync(OrderDto dto)
    {
        await this._capPublisher.PublishAsync(SalesMessageTopics.OrderCanceled, dto);
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