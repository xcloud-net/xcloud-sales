using DotNetCore.CAP;
using XCloud.Sales.Application;
using XCloud.Sales.Data.Domain.Orders;
using XCloud.Sales.Service.Orders;

namespace XCloud.Sales.Queue.Subscribe;

[UnitOfWork]
public class OrderConsumer : SalesAppService, ICapSubscribe
{
    private readonly IOrderService _orderService;
    public OrderConsumer(
        IOrderService orderService)
    {
        this._orderService = orderService;
    }

    [CapSubscribe(SalesMessageTopics.InsertOrderNote)]
    public virtual async Task InsertOrderNote(OrderNote message)
    {
        if (message == null)
            return;
            
        await this._orderService.InsertOrderNoteAsync(message);
    }
}