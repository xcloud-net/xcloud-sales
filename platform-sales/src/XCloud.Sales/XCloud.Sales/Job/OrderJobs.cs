using XCloud.Logging;
using XCloud.Sales.Application;
using XCloud.Sales.Service.Orders;

namespace XCloud.Sales.Job;

[LogExceptionSilence]
[UnitOfWork]
public class OrderJobs : SalesAppService, ITransientDependency
{
    private readonly IOrderProcessingService _orderProcessingService;
    private readonly IOrderShipmentProcessingService _orderShipmentProcessingService;
    private readonly IOrderService _orderService;

    public OrderJobs(IOrderProcessingService orderProcessingService,
        IOrderService orderService, 
        IOrderShipmentProcessingService orderShipmentProcessingService)
    {
        this._orderProcessingService = orderProcessingService;
        this._orderService = orderService;
        _orderShipmentProcessingService = orderShipmentProcessingService;
    }

    [LogExceptionSilence]
    public virtual async Task CancelUnpaidOrdersAsync()
    {
        await _orderProcessingService.CancelUnpaidOrderAsync();
    }

    [LogExceptionSilence]
    public virtual async Task AutoConfirmShippedOrderAsync()
    {
        await _orderShipmentProcessingService.AutoConfirmShippedOrderAsync();
    }
}