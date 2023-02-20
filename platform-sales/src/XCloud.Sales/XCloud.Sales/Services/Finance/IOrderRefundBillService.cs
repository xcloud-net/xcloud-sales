using XCloud.Sales.Data;
using XCloud.Sales.Data.Domain.Finance;

namespace XCloud.Sales.Services.Finance;

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