using XCloud.Sales.Data.Domain.Orders;
using XCloud.Sales.Data.Domain.Shipping;

namespace XCloud.Sales.Services.Orders;

public static class OrderExtension
{
    public static OrderStatus GetOrderStatus(this Order order) => (OrderStatus)order.OrderStatusId;

    public static void SetOrderStatus(this Order order, OrderStatus status)
    {
        order.OrderStatusId = (int)status;
    }

    public static PaymentStatus GetPaymentStatus(this Order order) => (PaymentStatus)order.PaymentStatusId;

    public static void SetPaymentStatus(this Order order, PaymentStatus status)
    {
        order.PaymentStatusId = (int)status;
    }

    public static ShippingStatus GetShippingStatus(this Order order) => (ShippingStatus)order.ShippingStatusId;

    public static void SetShippingStatus(this Order order, ShippingStatus status)
    {
        order.ShippingStatusId = (int)status;
    }
}