using XCloud.Sales.Data.Domain.Aftersale;
using XCloud.Sales.Service.Orders;

namespace XCloud.Sales.Service.AfterSale;

[ExposeServices(typeof(AfterSaleUtils))]
public class AfterSaleUtils : ITransientDependency
{
    private readonly OrderUtils _orderUtils;

    public AfterSaleUtils(OrderUtils orderUtils)
    {
        _orderUtils = orderUtils;
    }

    public int[] DoneStatus()
    {
        var status = new[]
        {
            AfterSalesStatus.Cancelled,
            AfterSalesStatus.Complete
        };

        return status.Select(x => (int)x).ToArray();
    }

    public int[] PendingStatus()
    {
        var status = new[]
        {
            AfterSalesStatus.None,
            AfterSalesStatus.Complete,
            AfterSalesStatus.Cancelled,
            AfterSalesStatus.Approved,
            AfterSalesStatus.Rejected,
            AfterSalesStatus.Processing
        };

        return status.Select(x => (int)x).Except(this.DoneStatus()).ToArray();
    }
}