using Stateless;
using XCloud.Core.Dto;
using XCloud.Database.EntityFrameworkCore.Extensions;
using XCloud.Redis;
using XCloud.Sales.Application;
using XCloud.Sales.Core;
using XCloud.Sales.Data;
using XCloud.Sales.Data.Domain.Orders;
using XCloud.Sales.Data.Domain.Shipping;

namespace XCloud.Sales.Service.Orders;

public interface IOrderShipmentProcessingService : ISalesAppService
{
    Task AutoConfirmShippedOrderAsync();

    Task MarkAsShippedAsync(IdDto dto);
}

public class OrderShipmentProcessingService : SalesAppService, IOrderShipmentProcessingService
{
    private readonly OrderUtils _orderUtils;
    private readonly ISalesRepository<Order> _orderRepository;
    private readonly IOrderProcessingService _orderProcessingService;

    public OrderShipmentProcessingService(OrderUtils orderUtils,
        ISalesRepository<Order> orderRepository, 
        IOrderProcessingService orderProcessingService)
    {
        this._orderUtils = orderUtils;
        this._orderRepository = orderRepository;
        _orderProcessingService = orderProcessingService;
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

    public virtual async Task MarkAsShippedAsync(IdDto dto)
    {
        var db = await this._orderRepository.GetDbContextAsync();

        var order = await this.GetOrderForUpdateAsync(db, dto.Id);

        var state = this.GetOrderStateMachine(order);

        if (!state.CanFire(OrderProcessingAction.Delivery))
            throw new SalesException("order can't be mark as finished");

        await state.FireAsync(OrderProcessingAction.Delivery);

        order.SetShippingStatus(ShippingStatus.Delivered);
        order.ShippingTime = this.Clock.Now;
        order.LastModificationTime = order.ShippingTime.Value;

        await db.TrySaveChangesAsync();

        await this.EventBusService.NotifyInsertOrderNote(new OrderNote()
        {
            OrderId = order.Id,
            Note = $"order has been shipped",
            DisplayToUser = false,
            CreationTime = this.Clock.Now
        });
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
                    await this._orderProcessingService.CancelAsync(new CancelOrderInput() { OrderId = m.Id, Comment = "auto cancel" });
            }
        }
        else
        {
            throw new FailToGetRedLockException("can't get lock");
        }
    }

    public virtual async Task AutoConfirmShippedOrderAsync()
    {
        using var dlock = await this.RedLockClient.RedLockFactory.CreateLockAsync(
            resource: $"{nameof(OrderProcessingService)}.{nameof(AutoConfirmShippedOrderAsync)}",
            expiryTime: TimeSpan.FromMinutes(3));

        if (dlock.IsAcquired)
        {
            var days = 7;
            var end = this.Clock.Now.AddDays(-days);

            var db = await this._orderRepository.GetDbContextAsync();
            var set = db.Set<Order>();

            var filteredStatus = new int[] { (int)OrderStatus.Delivering };

            while (true)
            {
                var query = set.AsNoTracking()
                    .Where(x => x.ShippingTime != null && x.ShippingTime.Value < end)
                    .Where(x => filteredStatus.Contains(x.OrderStatusId));

                var items = await query.Take(10).ToArrayAsync();
                if (!items.Any())
                    break;

                foreach (var m in items)
                {
                    await this._orderProcessingService.CompleteAsync(new CompleteOrderInput()
                        { OrderId = m.Id, Comment = "auto complete" });
                }
            }
        }
        else
        {
            throw new AbpException("can't get lock");
        }
    }
}