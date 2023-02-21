using XCloud.Sales.Application;
using XCloud.Sales.Data;
using XCloud.Sales.Data.Domain.Finance;

namespace XCloud.Sales.Service.Finance;

public interface IOrderRefundBillService : ISalesAppService
{
}

public class OrderRefundBillService : SalesAppService, IOrderRefundBillService
{
    private readonly ISalesRepository<OrderRefundBill> _salesRepository;

    public OrderRefundBillService(ISalesRepository<OrderRefundBill> salesRepository)
    {
        _salesRepository = salesRepository;
    }
}