using XCloud.Sales.Core;

namespace XCloud.Sales.Data.Domain.Orders;

public class OrderNote : SalesBaseEntity, IHasCreationTime
{
    public string OrderId { get; set; }

    public string Note { get; set; }

    public bool DisplayToUser { get; set; }

    public DateTime CreationTime { get; set; }

}