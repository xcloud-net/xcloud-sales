using Stateless;
using XCloud.Core.Dto;
using XCloud.Database.EntityFrameworkCore.Extensions;
using XCloud.Redis;
using XCloud.Sales.Core;
using XCloud.Sales.Data;
using XCloud.Sales.Data.Domain.Orders;
using XCloud.Sales.Data.Domain.Shipping;
using XCloud.Sales.Services.Catalog;
using XCloud.Sales.Services.Finance;
using XCloud.Sales.Services.Users;

namespace XCloud.Sales.Services.Orders;

public enum OrderProcessingAction : int
{
    PlaceOrder,
    Pay,
    Delivery,
    Close,
    Finish
}

public interface IOrderProcessingService : ISalesAppService
{
    Task<OrderBillDto> CreateOrderPayBillAsync(CreateOrderPayBillInput dto);

    Task DangerouslyUpdateStatusAsync(UpdateOrderStatusInput dto);

    Task AutoConfirmShippedOrderAsync();

    Task CancelUnpaidOrderAsync();

    Task CancelAsync(CancelOrderInput input);

    Task MarkAsShippedAsync(IdDto dto);

    Task CompleteAsync(CompleteOrderInput input);

    Task TrySetAsPaidAfterBillPaidAsync(IdDto input);

    Task MarkAsPaidAsync(MarkOrderAsPaidInput input);
}

public class OrderProcessingService : SalesAppService, IOrderProcessingService
{
    private readonly OrderUtils _orderUtils;
    private readonly IOrderBillService _orderBillService;
    private readonly IOrderService _orderService;
    private readonly IUserPointService _userPointService;
    private readonly ISalesRepository<Order> _orderRepository;
    private readonly IGoodsStockService _goodsStockService;

    public OrderProcessingService(IGoodsStockService goodsStockService,
        OrderUtils orderUtils,
        IOrderBillService orderBillService,
        IOrderService orderService,
        IUserPointService userPointService,
        ISalesRepository<Order> orderRepository)
    {
        this._goodsStockService = goodsStockService;
        this._orderUtils = orderUtils;
        this._orderBillService = orderBillService;
        this._orderRepository = orderRepository;
        this._orderService = orderService;
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
                    await this.CompleteAsync(new CompleteOrderInput()
                    { OrderId = m.Id, Comment = "auto complete" });
                }
            }
        }
        else
        {
            throw new AbpException("can't get lock");
        }
    }

    public async Task<OrderBillDto> CreateOrderPayBillAsync(CreateOrderPayBillInput dto)
    {
        var order = await this._orderService.QueryByIdAsync(dto.Id);
        if (order == null)
            throw new EntityNotFoundException();

        var bills = await this._orderBillService.QueryByOrderIdAsync(new ListOrderBillInput()
        { Id = dto.Id, Paid = true });

        var payedSum = bills.Sum(x => x.Price);

        if (payedSum > order.OrderTotal)
            throw new UserFriendlyException("this order is overpayed,pls start refund progress");

        if (this._orderUtils.MoneyEqual(payedSum, order.OrderTotal))
            throw new UserFriendlyException("order is payed,if you don't see that,pls refresh");

        var restPrice = order.OrderTotal - payedSum;

        var billPrice = dto.Price ?? restPrice;

        if (billPrice > restPrice)
            throw new AbpException("bill price is bigger than the rest price");

        var billRequest = new OrderBillDto()
        {
            OrderId = order.Id,
            Price = billPrice,
            PaymentMethod = (int)PaymentMethod.Manual,
        };

        var bill = await this._orderBillService.CreateBillAsync(billRequest);

        return bill;
    }

    [Obsolete]
    Task TrySetAsPaidAfterBillPaidImplAsync(IdDto input)
    {
        throw new NotImplementedException();
    }

    public virtual async Task TrySetAsPaidAfterBillPaidAsync(IdDto input)
    {
        var resourceKey = $"{nameof(TrySetAsPaidAfterBillPaidAsync)}.order.id.{input.Id}";

        using var dlock =
            await this.RedLockClient.RedLockFactory.CreateLockAsync(resourceKey,
                expiryTime: TimeSpan.FromSeconds(5));

        if (dlock.IsAcquired)
        {
            var now = this.Clock.Now;

            var db = await this._orderRepository.GetDbContextAsync();

            var order = await this.GetOrderForUpdateAsync(db, input.Id);

            if (order.GetOrderStatus() != OrderStatus.Pending)
                return;

            if (order.GetPaymentStatus() == PaymentStatus.Paid)
                return;

            var bills = await this._orderBillService.QueryByOrderIdAsync(new ListOrderBillInput()
            { Id = order.Id, Paid = true });

            var paidSum = bills.Sum(x => x.Price);

            order.SetPaymentStatus(PaymentStatus.Pending);
            order.OverPaid = paidSum > order.OrderTotal;
            if (_orderUtils.MoneyEqual(paidSum, order.OrderTotal) || order.OverPaid)
            {
                var lastBill = bills.MaxBy(x => x.CreationTime);

                order.SetPaymentStatus(PaymentStatus.Paid);
                order.PaidTime = lastBill?.PayTime ?? now;
            }
            else
            {
                if (paidSum > 0)
                {
                    order.SetPaymentStatus(PaymentStatus.PartiallyPaid);
                }
            }

            var state = this.GetOrderStateMachine(order);

            if (!state.CanFire(OrderProcessingAction.Pay))
                throw new SalesException("You can't mark this order as paid");

            //finish payment,continue
            if (order.GetPaymentStatus() == PaymentStatus.Paid)
            {
                await state.FireAsync(OrderProcessingAction.Pay);
            }

            order.LastModificationTime = now;

            await db.TrySaveChangesAsync();

            await this.EventBusService.NotifyInsertOrderNote(new OrderNote()
            {
                OrderId = order.Id,
                Note =
                    $"order payment status is update to {Enum.GetName(typeof(PaymentStatus), order.PaymentStatusId)}",
                DisplayToUser = false,
                CreationTime = this.Clock.Now
            });
        }
        else
        {
            throw new AbpException($"{nameof(TrySetAsPaidAfterBillPaidAsync)}.unable to get dlock");
        }
    }

    public virtual async Task MarkAsPaidAsync(MarkOrderAsPaidInput input)
    {
        var db = await this._orderRepository.GetDbContextAsync();

        var order = await this.GetOrderForUpdateAsync(db, input.OrderId);

        var state = this.GetOrderStateMachine(order);

        if (!state.CanFire(OrderProcessingAction.Pay))
            throw new SalesException("You can't mark this order as paid");

        await state.FireAsync(OrderProcessingAction.Pay);

        order.SetPaymentStatus(PaymentStatus.Paid);
        order.PaidTime = this.Clock.Now;
        order.LastModificationTime = order.PaidTime.Value;

        await this._orderRepository.UpdateAsync(order);

        await this.EventBusService.NotifyInsertOrderNote(new OrderNote()
        {
            OrderId = order.Id,
            Note = "Order.Message.OrderMarkedPaid",
            DisplayToUser = false,
            CreationTime = this.Clock.Now
        });
    }
}