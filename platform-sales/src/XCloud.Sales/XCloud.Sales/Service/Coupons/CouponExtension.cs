using XCloud.Sales.Data.Domain.Coupons;

namespace XCloud.Sales.Service.Coupons;

public static class CouponExtension
{
    public static bool AccountLimitationIsSet(this Coupon m, out int limitation)
    {
        limitation = default;
        if (m.AccountIssuedLimitCount != null && m.AccountIssuedLimitCount.Value > 0)
        {
            limitation = m.AccountIssuedLimitCount.Value;
            return true;
        }
        else
        {
            return false;
        }
    }
}