using Stateless;
using Volo.Abp.Timing;
using XCloud.Core.Dto;
using XCloud.Platform.Shared.Dto;
using XCloud.Sales.Clients.Platform;
using XCloud.Sales.Data.Domain.Orders;

namespace XCloud.Sales.Service.Orders;

public enum OrderProcessingAction : int
{
    PlaceOrder,
    Pay,
    Delivery,
    Close,
    Finish,
    FinishWithAfterSale
}

[ExposeServices(typeof(OrderUtils))]
public class OrderUtils : ITransientDependency
{
    private readonly PlatformInternalService _platformInternalService;
    private readonly IClock _clock;

    public OrderUtils(PlatformInternalService platformInternalService, IClock clock)
    {
        _platformInternalService = platformInternalService;
        _clock = clock;
    }

    /// <summary>
    /// 202301-0000001
    /// 100_0000 max
    /// </summary>
    public async Task<string> GenerateOrderSnAsync()
    {
        //date format: yyyyMMdd
        string MonthString() => this._clock.Now.ToString("yyyyMM");

        string PadLeft(int sn, int width)
        {
            var originSn = sn.ToString();
            var padLength = Math.Max(originSn.Length, width);
            return originSn.PadLeft(totalWidth: padLength, paddingChar: '0');
        }

        var month = MonthString();

        var response =
            await this._platformInternalService.GenerateSerialNoAsync(
                new CreateNoByCategoryDto($"order-sn-{month}"));

        if (!response.IsSuccess())
            throw new AbpException($"failed to generate order sn:{response.Error.Message}");

        if (response.Data >= 100_0000)
            throw new BusinessException(message: "order code is too large,in this month");

        var code = PadLeft(response.Data, 6);

        return $"{month}-{code}";
    }


    public StateMachine<OrderStatus, OrderProcessingAction> GetOrderStateMachine(Order order)
    {
        var state = new StateMachine<OrderStatus, OrderProcessingAction>(
            order.GetOrderStatus,
            order.SetOrderStatus);

        state.Configure(OrderStatus.None)
            .Permit(OrderProcessingAction.PlaceOrder, OrderStatus.Pending);

        state.Configure(OrderStatus.Pending)
            .Permit(OrderProcessingAction.Pay, OrderStatus.Processing)
            .Permit(OrderProcessingAction.Close, OrderStatus.Cancelled);

        state.Configure(OrderStatus.Processing)
            .Permit(OrderProcessingAction.Close, OrderStatus.Cancelled)
            .Permit(OrderProcessingAction.Delivery, OrderStatus.Delivering)
            .Permit(OrderProcessingAction.Finish, OrderStatus.Complete);

        state.Configure(OrderStatus.Delivering)
            .Permit(OrderProcessingAction.Close, OrderStatus.Cancelled)
            .Permit(OrderProcessingAction.Finish, OrderStatus.Complete)
            .Permit(OrderProcessingAction.FinishWithAfterSale, OrderStatus.FinishWithAfterSale);

        //no further flows
        state.Configure(OrderStatus.Complete);

        //no further flows
        state.Configure(OrderStatus.Cancelled);

        //no further flows
        state.Configure(OrderStatus.FinishWithAfterSale);

        return state;
    }

    public int[] DoneStatus()
    {
        var status = new[] { OrderStatus.Cancelled, OrderStatus.Complete, OrderStatus.FinishWithAfterSale };
        return status.Select(x => (int)x).ToArray();
    }

    public bool IsInAfterSalePending(Order order)
    {
        return order.IsAftersales;
    }

    public async Task EnsureOrderProcessableAsync(Order order)
    {
        if (order == null)
            throw new EntityNotFoundException("Order cannot be loaded");

        if (order.IsDeleted)
            throw new UserFriendlyException("order is deleted");

        if (IsInAfterSalePending(order))
            throw new UserFriendlyException("order is in after sale");

        await Task.CompletedTask;
    }

    public bool IsMoneyEqual(decimal price1, decimal price2) => (int)(price1 * 100) == (int)(price2 * 100);

    public bool IsOrderRefundable(Order order)
    {
        var status = new[]
        {
            OrderStatus.Processing,
            OrderStatus.Delivering,
            OrderStatus.Complete,
        };

        return status.Contains(order.GetOrderStatus());
    }
    
    public async Task CalculateOrderFinalPriceAsync(Order order, OrderItem[] orderItems)
    {
        //todo put into order utils

        order.GradePriceOffsetTotal = orderItems.Sum(x => x.GradePriceOffset);
        order.OrderSubtotal = orderItems.Sum(x => x.Price);
        //total
        order.OrderTotal = order.OrderSubtotal + order.OrderShippingFee;
        order.OrderTotal -= order.CouponDiscount;
        order.OrderTotal -= order.PromotionDiscount;
        order.ExchangePointsAmount = order.OrderTotal;

        await Task.CompletedTask;
    }
}