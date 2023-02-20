using Microsoft.AspNetCore.Mvc;
using XCloud.Core.Dto;
using XCloud.Sales.Services.Coupons;

namespace XCloud.Sales.Mall.Api.Controllers;

[Route("api/mall/coupon")]
public class CouponController : ShopBaseController
{
    private readonly ICouponService _couponService;

    public CouponController(ICouponService couponService)
    {
        this._couponService = couponService;
    }

    [Obsolete]
    [HttpPost("paging")]
    public async Task<PagedResponse<CouponDto>> QueryCouponPagingAsync([FromBody] QueryCouponPagingInput dto)
    {
        dto.IsDeleted = false;
        dto.AvailableFor = this.Clock.Now;
        dto.SkipCalculateTotalCount = true;

        var response = await this._couponService.QueryPagingAsync(dto);

        var storeUserOrNull = await this.StoreAuthService.GetStoreUserOrNullAsync();
        if (storeUserOrNull != null && response.IsNotEmpty)
        {
            await this._couponService.AttachDataAsync(response.Items.ToArray(), new AttachCouponDataInput()
            {
                IssuedToUserId = storeUserOrNull.Id
            });
        }

        return response;
    }
    
    [HttpPost("user-coupon-paging")]
    public async Task<PagedResponse<CouponUserMappingDto>> QueryUserCouponPagingAsync([FromBody] QueryUserCouponPagingInput dto)
    {
        var storeUser = await this.StoreAuthService.GetRequiredStoreUserAsync();
        
        dto.IsDeleted = false;
        dto.UserId = storeUser.Id;
        dto.AvailableFor = this.Clock.Now;
        dto.Used = false;
        dto.SkipCalculateTotalCount = true;

        var response = await this._couponService.QueryUserCouponPagingAsync(dto);

        return response;
    }

    [HttpPost("issue-coupon")]
    public async Task<ApiResponse<object>> IssueCouponAsync([FromBody] IssueCouponInput dto)
    {
        var storeUser = await this.StoreAuthService.GetRequiredStoreUserAsync();

        dto.UserId = storeUser.Id;

        await this._couponService.IssueCouponAsync(dto.CouponId, dto.UserId);

        return new ApiResponse<object>();
    }
}