using XCloud.Sales.Core;

namespace XCloud.Sales.Data.Domain.Finance;

public class OrderRefundBill : SalesBaseEntity<string>, IHasDeletionTime, IHasModificationTime, IHasCreationTime
{
    public string OrderId { get; set; }
    public string BillId { get; set; }

    public decimal Price { get; set; }

    public bool Approved { get; set; }
    public DateTime? ApprovedTime { get; set; }

    public bool Refunded { get; set; }
    public DateTime? RefundTime { get; set; }
    public string RefundTransactionId { get; set; }
    public string RefundNotifyData { get; set; }

    public DateTime? LastModificationTime { get; set; }

    public bool IsDeleted { get; set; }
    public DateTime? DeletionTime { get; set; }
    
    public DateTime CreationTime { get; set; }
}