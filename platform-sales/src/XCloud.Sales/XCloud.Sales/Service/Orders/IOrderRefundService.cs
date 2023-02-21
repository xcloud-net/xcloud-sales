using XCloud.Sales.Application;
using XCloud.Sales.Data.Domain.Orders;
using XCloud.Sales.Service.Coupons;
using XCloud.Sales.Service.Finance;
using XCloud.Sales.Service.Users;

namespace XCloud.Sales.Service.Orders
{
    public interface IOrderRefundService : ISalesAppService
    {
        Task<OrderCanBeRefundResponse> OrderCanBeRefundAsync(string orderId);

        Task RefundUserBalancePaidBillAsync(string billId);
    }

    public class OrderRefundService : SalesAppService, IOrderRefundService
    {
        private readonly ICouponService _couponService;
        private readonly IUserBalanceService _userBalanceService;
        private readonly IOrderBillService _orderBillService;
        private readonly IOrderService _orderService;

        public OrderRefundService(IOrderService orderService, ICouponService couponService,
            IOrderBillService orderBillService, IUserBalanceService userBalanceService)
        {
            this._orderService = orderService;
            this._orderBillService = orderBillService;
            this._userBalanceService = userBalanceService;
            this._couponService = couponService;
        }

        public async Task<OrderCanBeRefundResponse> OrderCanBeRefundAsync(string orderId)
        {
            var order = await this._orderService.QueryByIdAsync(orderId);
            if (order == null)
                throw new EntityNotFoundException(nameof(order));

            throw new NotImplementedException();
        }

        private async Task<OrderDto> GetRequiredOrderToRefundAsync(string orderId)
        {
            var order = await this._orderService.QueryByIdAsync(orderId);
            if (order == null)
                throw new EntityNotFoundException(nameof(order));

            var dto = this.ObjectMapper.Map<Order, OrderDto>(order);

            return dto;
        }

        public async Task RefundUserBalancePaidBillAsync(string billId)
        {
            var bill = await this._orderBillService.QueryByIdAsync(billId);

            if (bill == null)
                throw new EntityNotFoundException(nameof(bill));

            if (bill.BillPaymentMethod != PaymentMethod.Balance)
                throw new NotSupportedException("wrong payment method");

            if (bill.Refunded ?? false)
                throw new AbpException("bill has been refunded already");

            var order = await this._orderService.QueryByIdAsync(bill.OrderId);
            if (order == null)
                throw new EntityNotFoundException(nameof(order));

            await this._orderBillService.MarkBillAsRefundAsync(new MarkBillAsRefundInput()
            {
                Id = bill.Id,
                TransactionId = default,
                NotifyData = default
            });

            await this._userBalanceService.ChangeUserBalanceAsync(
                userId: order.UserId, amount: bill.Price,
                BalanceActionType.Add, "order refund");
        }
    }
}

