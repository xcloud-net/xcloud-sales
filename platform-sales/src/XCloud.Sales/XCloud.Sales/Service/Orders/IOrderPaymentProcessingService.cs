using Stateless;
using XCloud.Core.Dto;
using XCloud.Database.EntityFrameworkCore.Extensions;
using XCloud.Sales.Application;
using XCloud.Sales.Core;
using XCloud.Sales.Data;
using XCloud.Sales.Data.Domain.Orders;
using XCloud.Sales.Service.Finance;

namespace XCloud.Sales.Service.Orders;

public interface IOrderPaymentProcessingService : ISalesAppService
{
    Task<OrderBillDto> CreateOrderPayBillAsync(CreateOrderPayBillInput dto);

    Task TrySetAsPaidAfterBillPaidAsync(IdDto input);

    Task MarkAsPaidAsync(MarkOrderAsPaidInput input);
}

public class OrderPaymentProcessingService : SalesAppService, IOrderPaymentProcessingService
{
    private readonly OrderUtils _orderUtils;
    private readonly IOrderBillService _orderBillService;
    private readonly IOrderService _orderService;
    private readonly ISalesRepository<Order> _orderRepository;

    public OrderPaymentProcessingService(OrderUtils orderUtils,
        IOrderBillService orderBillService,
        IOrderService orderService,
        ISalesRepository<Order> orderRepository)
    {
        this._orderUtils = orderUtils;
        this._orderBillService = orderBillService;
        this._orderRepository = orderRepository;
        this._orderService = orderService;
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

        if (this._orderUtils.IsMoneyEqual(payedSum, order.OrderTotal))
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
            if (_orderUtils.IsMoneyEqual(paidSum, order.OrderTotal) || order.OverPaid)
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