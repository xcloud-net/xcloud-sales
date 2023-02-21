using Stateless;
using XCloud.Sales.Data.Domain.Orders;

namespace XCloud.Sales.Service.Orders;

public enum OrderProcessingAction : int
{
    PlaceOrder,
    Pay,
    Delivery,
    Close,
    Finish
}

[ExposeServices(typeof(OrderUtils))]
public class OrderUtils : ITransientDependency
{
    public OrderUtils()
    {
        //
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
            .Permit(OrderProcessingAction.Finish, OrderStatus.Complete);

        state.Configure(OrderStatus.Complete);

        state.Configure(OrderStatus.Cancelled);

        return state;
    }

    public int[] DoneStatus()
    {
        var status = new[] { OrderStatus.Cancelled, OrderStatus.Complete, OrderStatus.AfterSale };
        return status.Select(x => (int)x).ToArray();
    }

    public async Task EnsureOrderCanBeTakeActionAsync(Order order)
    {
        if (order == null)
            throw new EntityNotFoundException("Order cannot be loaded");

        if (order.IsDeleted)
            throw new UserFriendlyException("order is deleted");

        if (order.IsAftersales || !string.IsNullOrWhiteSpace(order.AfterSalesId))
            throw new UserFriendlyException("order is in after sale");

        await Task.CompletedTask;
    }

    public bool MoneyEqual(decimal price1, decimal price2) => (int)(price1 * 100) == (int)(price2 * 100);
}