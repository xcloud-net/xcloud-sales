using XCloud.Sales.Core;

namespace XCloud.Sales.Data.Domain.Finance;

public class OrderBill : SalesBaseEntity<string>, ISoftDelete, IHasCreationTime
{
    public OrderBill()
    {
        //
    }

    //use id instead
    //public string OutTradeNo { get; set; }
    public string OrderId { get; set; }
    public decimal Price { get; set; }

    public int PaymentMethod { get; set; }

    //payment
    public bool Paid { get; set; } = false;
    public DateTime? PayTime { get; set; }
    public string PaymentTransactionId { get; set; }

    public string NotifyData { get; set; }

    //refund
    public bool? Refunded { get; set; }
    public DateTime? RefundTime { get; set; }
    public string RefundTransactionId { get; set; }
    public string RefundNotifyData { get; set; }

    public bool IsDeleted { get; set; }
    public DateTime CreationTime { get; set; }
}