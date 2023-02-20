namespace XCloud.Sales.Data.Domain.Aftersale;

public enum AfterSalesStatus : int
{
    None = 0,

    Processing = 1,

    Approved = 2,

    Rejected = 3,

    Complete = 4,

    Cancelled = 5,
}