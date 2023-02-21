using Stateless;
using XCloud.Database.EntityFrameworkCore.Extensions;
using XCloud.Redis;
using XCloud.Sales.Application;
using XCloud.Sales.Core;
using XCloud.Sales.Data;
using XCloud.Sales.Data.Domain.Orders;
using XCloud.Sales.Service.Catalog;
using XCloud.Sales.Service.Users;

namespace XCloud.Sales.Service.Orders;

public interface IOrderProcessingService : ISalesAppService
{
    Task<OrderProcessingAction[]> GetAvailableActionsAsync(Order order);
    
    Task DangerouslyUpdateStatusAsync(UpdateOrderStatusInput dto);

    Task CancelUnpaidOrderAsync();

    Task CancelAsync(CancelOrderInput input);

    Task CompleteAsync(CompleteOrderInput input);
}

public class OrderProcessingService : SalesAppService, IOrderProcessingService
{
    private readonly OrderUtils _orderUtils;
    private readonly IUserPointService _userPointService;
    private readonly ISalesRepository<Order> _orderRepository;
    private readonly IGoodsStockService _goodsStockService;

    public OrderProcessingService(IGoodsStockService goodsStockService,
        OrderUtils orderUtils,
        IUserPointService userPointService,
        ISalesRepository<Order> orderRepository)
    {
        this._goodsStockService = goodsStockService;
        this._orderUtils = orderUtils;
        this._orderRepository = orderRepository;
        this._userPointService = userPointService;
    }

    private StateMachine<OrderStatus, OrderProcessingAction> GetOrderStateMachine(Order order)
    {
        return this._orderUtils.GetOrderStateMachine(order);
    }

    private async Task<Order> GetOrderForUpdateAsync(DbContext db, string orderId)
    {
        var order = await db.Set<Order>().FirstOrDefaultAsync(x => x.Id == orderId);

        await this._orderUtils.EnsureOrderCanBeTakeActionAsync(order);

        return order;
    }

    public async Task<OrderProcessingAction[]> GetAvailableActionsAsync(Order order)
    {
        await Task.CompletedTask;

        var state = this.GetOrderStateMachine(order);
        var triggers = state.GetPermittedTriggers().ToArray();

        return triggers;
    }

    public virtual async Task CompleteAsync(CompleteOrderInput input)
    {
        var db = await this._orderRepository.GetDbContextAsync();

        var order = await this.GetOrderForUpdateAsync(db, input.OrderId);

        var state = this.GetOrderStateMachine(order);

        if (!state.CanFire(OrderProcessingAction.Finish))
            throw new SalesException("order can't be mark as finished");

        await state.FireAsync(OrderProcessingAction.Finish);
        order.LastModificationTime = this.Clock.Now;

        await db.TrySaveChangesAsync();

        await this._userPointService.AddPointsHistoryAsync(new PointsHistoryDto()
        {
            UserId = order.UserId,
            OrderId = order.Id,
            Points = (int)order.OrderTotal,
            ActionType = (int)PointsActionType.Add,
            Message = "from order",
        });

        await this.EventBusService.NotifyInsertOrderNote(new OrderNote()
        {
            OrderId = order.Id,
            Note = $"Order.Message.OrderStatusChanged:{input.Comment}",
            DisplayToUser = false,
            CreationTime = this.Clock.Now
        });
    }

    public virtual async Task DangerouslyUpdateStatusAsync(UpdateOrderStatusInput dto)
    {
        if (dto == null || string.IsNullOrWhiteSpace(dto.Id))
            throw new ArgumentNullException(nameof(DangerouslyUpdateStatusAsync));

        var db = await this._orderRepository.GetDbContextAsync();

        var order = await this.GetOrderForUpdateAsync(db, dto.Id);

        if (dto.OrderStatus != null)
            order.OrderStatusId = dto.OrderStatus.Value;

        if (dto.PaymentStatus != null)
            order.PaymentStatusId = dto.PaymentStatus.Value;

        if (dto.DeliveryStatus != null)
            order.ShippingStatusId = dto.DeliveryStatus.Value;

        order.LastModificationTime = this.Clock.Now;

        await db.TrySaveChangesAsync();
    }

    public virtual async Task CancelAsync(CancelOrderInput input)
    {
        var db = await this._orderRepository.GetDbContextAsync();

        var order = await this.GetOrderForUpdateAsync(db, input.OrderId);

        var state = this.GetOrderStateMachine(order);

        if (!state.CanFire(OrderProcessingAction.Close))
            throw new SalesException("Cannot do cancel for order.");

        await state.FireAsync(OrderProcessingAction.Close);
        order.LastModificationTime = this.Clock.Now;

        await db.TrySaveChangesAsync();

        await this.EventBusService.NotifyInsertOrderNote(new OrderNote()
        {
            OrderId = order.Id,
            Note = $"order is canceled:{input.Comment}",
            DisplayToUser = false,
            CreationTime = this.Clock.Now
        });

        var items = await db.Set<OrderItem>().Where(x => x.OrderId == order.Id).ToArrayAsync();

        foreach (var m in items)
        {
            await this._goodsStockService.AdjustCombinationStockAsync(m.GoodsSpecCombinationId, m.Quantity);
        }
    }

    public virtual async Task CancelUnpaidOrderAsync()
    {
        using var dlock = await this.RedLockClient.RedLockFactory.CreateLockAsync(
            resource: $"{nameof(OrderProcessingService)}.{nameof(CancelUnpaidOrderAsync)}",
            expiryTime: TimeSpan.FromMinutes(3));

        if (dlock.IsAcquired)
        {
            var minutes = 10;
            var end = this.Clock.Now.AddMinutes(-minutes);

            var db = await this._orderRepository.GetDbContextAsync();
            var set = db.Set<Order>();

            var filteredStatus = new int[] { (int)OrderStatus.Pending };
            var nonePaymentStatus = (int)PaymentStatus.Pending;

            while (true)
            {
                var query = set.AsNoTracking()
                    .Where(x => x.CreationTime < end)
                    .Where(x => filteredStatus.Contains(x.OrderStatusId))
                    .Where(x => x.PaymentStatusId == nonePaymentStatus);

                var items = await query.Take(10).ToArrayAsync();
                if (!items.Any())
                    break;

                foreach (var m in items)
                    await this.CancelAsync(new CancelOrderInput() { OrderId = m.Id, Comment = "auto cancel" });
            }
        }
        else
        {
            throw new FailToGetRedLockException("can't get lock");
        }
    }
}