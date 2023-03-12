using Volo.Abp.Application.Dtos;
using XCloud.Core.Dto;
using XCloud.Sales.Data.Domain.Finance;
using XCloud.Sales.Data.Domain.Orders;
using XCloud.Sales.Service.Orders;

namespace XCloud.Sales.Service.Finance;

public class UpdateRefundBillStatus : IEntityDto<string>
{
    public string Id { get; set; }
    public bool? IsDeleted { get; set; }
}

public class ApproveRefundBillInput : IEntityDto<string>
{
    public string Id { get; set; }
    public string StoreManagerId { get; set; }
    public string Comment { get; set; }
}

public class OrderBillDto : OrderBill, IEntityDto
{
    public OrderDto Order { get; set; }

    public PaymentMethod BillPaymentMethod
    {
        get => (PaymentMethod)this.PaymentMethod;
        set { this.PaymentMethod = (int)value; }
    }
}

public class OrderRefundBillDto : OrderRefundBill, IEntityDto<string>
{
    public OrderBillDto OrderBill { get; set; }

    public OrderDto Order { get; set; }
}

public class AttachOrderBillDataInput : IEntityDto
{
    public bool Order { get; set; } = false;
}

public class QueryOrderBillPagingInput : PagedRequest, IEntityDto
{
    public string OrderId { get; set; }

    public string OrderNo { get; set; }

    public int? PaymentMethod { get; set; }

    public bool? Paid { get; set; }

    public string PaymentTransactionId { get; set; }

    public string OutTradeNo { get; set; }

    public string RefundTransactionId { get; set; }

    public bool? Refunded { get; set; }
}

public class ListOrderBillInput : IEntityDto<string>
{
    public string Id { get; set; }
    public int? PaymentMethod { get; set; }
    public bool? Paid { get; set; }
    public int? MaxCount { get; set; }
}

public class MarkBillAsPayedInput : IEntityDto<string>
{
    public string Id { get; set; }
    public int PaymentMethod { get; set; }
    public string TransactionId { get; set; }
    public string NotifyData { get; set; }
}

public class AttachRefundBillDataInput : IEntityDto
{
    public bool OrderBill { get; set; } = false;
}

public class QueryRefundBillPagingInput : PagedRequest, IEntityDto
{
    //
}

public class MarkRefundBillAsRefundInput : IEntityDto<string>
{
    public string Id { get; set; }
    public string TransactionId { get; set; }
    public string NotifyData { get; set; }
}

[Obsolete]
public class MarkBillAsRefundInput : IEntityDto<string>
{
    public string Id { get; set; }
    public string TransactionId { get; set; }
    public string NotifyData { get; set; }
}