namespace XCloud.Sales.Data.Domain.Orders;

public enum PaymentMethod : int
{
    None = 0,
    Manual = 1,
    Balance = 2,
    WechatMp = 3,
    WechatOpen = 4,
}

public enum PaymentStatus : int
{
    Pending = 0,

    //Voided = 1,

    PartiallyPaid = 2,

    Paid = 3,

    PartiallyRefunded = 4,

    Refunded = 5,
}