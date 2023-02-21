using Microsoft.AspNetCore.Mvc;
using XCloud.Core.Dto;
using XCloud.Sales.Service.Authentication;
using XCloud.Sales.Service.Coupons;

namespace XCloud.Sales.Mall.Api.Controllers.Admin;

[StoreAuditLog]
[Route("api/mall-admin/coupon")]
public class CouponController : ShopBaseController
{
    private readonly ICouponService _couponService;

    public CouponController(ICouponService couponService)
    {
        this._couponService = couponService;
    }

    [HttpPost("paging")]
    public async Task<PagedResponse<CouponDto>> QueryCouponPagingAsync([FromBody] QueryCouponPagingInput dto)
    {
        var storeAdministrator = await this.StoreAuthService.GetRequiredStoreAdministratorAsync();

        await this.SalesPermissionService.CheckRequiredPermissionAsync(storeAdministrator,
            SalesPermissions.ManageCoupons);

        dto.IsDeleted = null;
        dto.SortForAdmin = true;
        var response = await this._couponService.QueryPagingAsync(dto);

        return response;
    }

    [HttpPost("create-user-coupon")]
    public async Task<ApiResponse<object>> CreateUserCouponAsync([FromBody] CouponUserMappingDto dto)
    {
        var storeAdministrator = await this.StoreAuthService.GetRequiredStoreAdministratorAsync();

        await this.SalesPermissionService.CheckRequiredPermissionAsync(storeAdministrator,
            SalesPermissions.ManageCoupons);

        await this._couponService.CreateUserCouponAsync(dto);
        
        return new ApiResponse<object>();
    }

    [HttpPost("user-coupon-paging")]
    public async Task<PagedResponse<CouponUserMappingDto>> QueryUserCouponPagingAsync(
        [FromBody] QueryUserCouponPagingInput dto)
    {
        var storeAdministrator = await this.StoreAuthService.GetRequiredStoreAdministratorAsync();

        await this.SalesPermissionService.CheckRequiredPermissionAsync(storeAdministrator,
            SalesPermissions.ManageCoupons);

        dto.IsDeleted = null;

        var response = await this._couponService.QueryUserCouponPagingAsync(dto);

        if (response.IsNotEmpty)
        {
            await this._couponService.AttachUserCouponDataAsync(response.Items.ToArray(),
                new AttachUserCouponDataInput()
                {
                    User = true
                });
        }

        return response;
    }

    [Obsolete]
    [HttpPost("create")]
    public async Task<ApiResponse<object>> CreateCouponAsync([FromBody] CouponDto dto)
    {
        var storeAdministrator = await this.StoreAuthService.GetRequiredStoreAdministratorAsync();

        await this.SalesPermissionService.CheckRequiredPermissionAsync(storeAdministrator,
            SalesPermissions.ManageCoupons);

        await this._couponService.InsertCouponAsync(dto);

        return new ApiResponse<object>();
    }

    [HttpPost("save")]
    public virtual async Task<ApiResponse<object>> SaveCouponAsync([FromBody] CouponDto dto)
    {
        var storeAdministrator = await this.StoreAuthService.GetRequiredStoreAdministratorAsync();

        await this.SalesPermissionService.CheckRequiredPermissionAsync(storeAdministrator,
            SalesPermissions.ManageCoupons);

        if (dto.Id <= 0)
        {
            await this._couponService.InsertCouponAsync(dto);
        }
        else
        {
            await this._couponService.UpdateCouponAsync(dto);
        }

        return new ApiResponse<object>();
    }

    [HttpPost("update-status")]
    public async Task<ApiResponse<object>> UpdateStatusAsync([FromBody] UpdateCouponStatusInput dto)
    {
        var storeAdministrator = await this.StoreAuthService.GetRequiredStoreAdministratorAsync();

        await this.SalesPermissionService.CheckRequiredPermissionAsync(storeAdministrator,
            SalesPermissions.ManageCoupons);

        await this._couponService.UpdateStatusAsync(dto);

        return new ApiResponse<object>();
    }
}