namespace XCloud.Sales.Data.Domain.Orders;

public enum OrderStatus : int
{
    None = 0,

    Pending = 10,

    Processing = 20,

    Delivering = 23,

    Complete = 30,

    Cancelled = 40,

    FinishWithAfterSale = 50,
}