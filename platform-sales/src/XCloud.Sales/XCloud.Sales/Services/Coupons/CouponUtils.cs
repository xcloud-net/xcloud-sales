using Volo.Abp.Timing;
using XCloud.Sales.Data.Domain.Coupons;

namespace XCloud.Sales.Services.Coupons;

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
}