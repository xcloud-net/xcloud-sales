using XCloud.Logging;
using XCloud.Sales.Services.Orders;

namespace XCloud.Sales.Job;

[LogExceptionSilence]
[UnitOfWork]
public class OrderJobs : SalesAppService, ITransientDependency
{
    private readonly IOrderProcessingService _orderProcessingService;
    private readonly IOrderService _orderService;

    public OrderJobs(IOrderProcessingService orderProcessingService,
        IOrderService orderService)
    {
        this._orderProcessingService = orderProcessingService;
        this._orderService = orderService;
    }

    [LogExceptionSilence]
    public virtual async Task CancelUnpaidOrdersAsync()
    {
        await _orderProcessingService.CancelUnpaidOrderAsync();
    }

    [LogExceptionSilence]
    public virtual async Task AutoConfirmShippedOrderAsync()
    {
        await _orderProcessingService.AutoConfirmShippedOrderAsync();
    }
}