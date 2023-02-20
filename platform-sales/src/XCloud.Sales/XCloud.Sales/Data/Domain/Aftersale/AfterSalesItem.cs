using XCloud.Sales.Core;

namespace XCloud.Sales.Data.Domain.Aftersale;

public class AftersalesItem : SalesBaseEntity<string>
{
    public string AftersalesId { get; set; }

    public string OrderId { get; set; }

    public int OrderItemId { get; set; }

    public int Quantity { get; set; }
}