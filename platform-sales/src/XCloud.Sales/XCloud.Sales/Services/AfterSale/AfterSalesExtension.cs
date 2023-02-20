using XCloud.Sales.Data.Domain.Aftersale;

namespace XCloud.Sales.Services.AfterSale;

public static class AfterSalesExtension
{
    public static void SetAfterSalesStatus(this AfterSales afterSales, AfterSalesStatus status)
    {
        afterSales.AfterSalesStatusId = (int)status;
    }

    public static AfterSalesStatus GetAfterSalesStatus(this AfterSales afterSales) =>
        (AfterSalesStatus)afterSales.AfterSalesStatusId;
}