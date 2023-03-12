using Volo.Abp.Timing;
using XCloud.Sales.Data.Domain.Coupons;
using XCloud.Sales.Data.Domain.Orders;

namespace XCloud.Sales.Service.Coupons;

[ExposeServices(typeof(CouponUtils))]
public class CouponUtils : ITransientDependency
{
    private readonly IClock _clock;

    public CouponUtils(IClock clock)
    {
        this._clock = clock;
    }

    public bool IsUserCouponExpired(CouponUserMapping userCoupon)
    {
        return userCoupon.ExpiredAt == null || userCoupon.ExpiredAt.Value > this._clock.Now;
    }

    public async Task ApplyToOrderAsync(Order order, CouponUserMapping userCoupon)
    {
        if (userCoupon.Value < 0)
            throw new BusinessException(message: "coupon value less than zero");

        order.CouponDiscount = userCoupon.Value;
        await Task.CompletedTask;
    }
}