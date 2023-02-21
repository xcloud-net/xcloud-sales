using XCloud.Sales.Service.Orders;

namespace XCloud.Sales.Core;

public static class SalesDbConnectionNames
{
    public static string SalesCloud => "XSales";
}

public static class SalesDefault
{
    public static OrderStockDeductStrategy OrderStockDeductStrategy => OrderStockDeductStrategy.AfterPlaceOrder;
}