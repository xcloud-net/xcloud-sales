using XCloud.Sales.Core;

namespace XCloud.Sales.Data.Domain.Finance;

public class OrderRefundBill : SalesBaseEntity<string>
{
    public string OrderId { get; set; }
    public string BillId { get; set; }
    public decimal Price { get; set; }
    public bool? Refunded { get; set; }
    public DateTime? RefundTime { get; set; }
    public string RefundTransactionId { get; set; }
    public string RefundNotifyData { get; set; }

}