using DotNetCore.CAP;
using XCloud.Sales.Application;
using XCloud.Sales.Data.Domain.Orders;
using XCloud.Sales.Service.Orders;

namespace XCloud.Sales.Queue.Subscribe;

[UnitOfWork]
public class OrderConsumer : SalesAppService, ICapSubscribe
{
    private readonly IOrderService orderService;
    public OrderConsumer(
        IOrderService orderService)
    {
        this.orderService = orderService;
    }

    [CapSubscribe(SalesMessageTopics.InsertOrderNote)]
    public virtual async Task InsertOrderNote(OrderNote message)
    {
        if (message == null)
            return;
            
        await this.orderService.InsertOrderNoteAsync(message);
    }
}