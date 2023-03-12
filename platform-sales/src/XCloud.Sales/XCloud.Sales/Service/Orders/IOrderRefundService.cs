using XCloud.Core.Dto;
using XCloud.Sales.Application;
using XCloud.Sales.Data.Domain.Orders;
using XCloud.Sales.Service.Coupons;
using XCloud.Sales.Service.Finance;
using XCloud.Sales.Service.Users;

namespace XCloud.Sales.Service.Orders
{
    public interface IOrderRefundService : ISalesAppService
    {
        Task RefundUserCouponAsync(string orderId);

        Task<OrderCanBeRefundResponse> OrderCanBeRefundAsync(string orderId);

        Task RefundUserBalancePaidBillAsync(string billId);

        Task RefundToWechatAsync(string billId);
    }

    public class OrderRefundService : SalesAppService, IOrderRefundService
    {
        private readonly ICouponService _couponService;
        private readonly IUserBalanceService _userBalanceService;
        private readonly IOrderBillService _orderBillService;
        private readonly IOrderService _orderService;
        private readonly OrderUtils _orderUtils;
        private readonly IOrderRefundBillService _orderRefundBillService;

        public OrderRefundService(IOrderService orderService, ICouponService couponService,
            IOrderBillService orderBillService, IUserBalanceService userBalanceService,
            OrderUtils orderUtils, IOrderRefundBillService orderRefundBillService)
        {
            this._orderService = orderService;
            this._orderBillService = orderBillService;
            this._userBalanceService = userBalanceService;
            _orderUtils = orderUtils;
            _orderRefundBillService = orderRefundBillService;
            this._couponService = couponService;
        }

        public async Task<OrderCanBeRefundResponse> OrderCanBeRefundAsync(string orderId)
        {
            var order = await this._orderService.QueryByIdAsync(orderId);
            if (order == null)
                throw new EntityNotFoundException(nameof(order));

            if (this._orderUtils.IsOrderRefundable(order))
            {
                return new OrderCanBeRefundResponse();
            }
            else
            {
                return new OrderCanBeRefundResponse().SetError("status error");
            }
        }

        public async Task RefundUserCouponAsync(string orderId)
        {
            if (string.IsNullOrWhiteSpace(orderId))
                throw new ArgumentNullException(nameof(orderId));

            var order = await this._orderService.QueryByIdAsync(orderId);

            if (order == null)
                throw new EntityNotFoundException(nameof(order));

            if (order.CouponId == null || order.CouponId <= 0)
                throw new BusinessException(message: "order coupon is empty");

            var coupon = await this._couponService.GetUserCouponByIdAsync(order.CouponId.Value);
            if (coupon == null)
                throw new EntityNotFoundException(nameof(coupon));

            await this._couponService.ReturnBackUserCouponAsync(coupon.Id);
        }

        public async Task RefundToWechatAsync(string billId)
        {
            throw new NotImplementedException();
        }

        public async Task RefundUserBalancePaidBillAsync(string billId)
        {
            if (string.IsNullOrWhiteSpace(billId))
                throw new ArgumentNullException(nameof(billId));
            
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