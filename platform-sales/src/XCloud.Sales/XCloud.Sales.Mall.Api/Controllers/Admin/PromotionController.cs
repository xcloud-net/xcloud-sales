using Microsoft.AspNetCore.Mvc;
using XCloud.Core.Dto;
using XCloud.Sales.Services.Promotion;

namespace XCloud.Sales.Mall.Api.Controllers.Admin;

[StoreAuditLog]
[Route("api/mall-admin/promotion")]
public class PromotionController : ShopBaseController
{
    private readonly IPromotionService _promotionService;

    public PromotionController(IPromotionService promotionService)
    {
        this._promotionService = promotionService;
    }

    [HttpPost("paging")]
    public async Task<PagedResponse<StorePromotionDto>> QueryCouponPagingAsync([FromBody] QueryPromotionPagingInput dto)
    {
        var storeAdministrator = await this.StoreAuthService.GetRequiredStoreAdministratorAsync();

        await this.SalesPermissionService.CheckRequiredPermissionAsync(storeAdministrator,
            SalesPermissions.ManagePromotion);

        dto.IsDeleted = null;
        dto.SortForAdmin = true;
        var response = await this._promotionService.QueryPagingAsync(dto);

        if (response.IsNotEmpty)
        {
            await this._promotionService.AttachDataAsync(response.Items.ToArray(), new AttachPromotionDataInput()
            {
                ParseCondition = true,
                ParseResults = true
            });
        }

        return response;
    }

    [Obsolete]
    [HttpPost("create")]
    public async Task<ApiResponse<object>> CreateCouponAsync([FromBody] StorePromotionDto dto)
    {
        var storeAdministrator = await this.StoreAuthService.GetRequiredStoreAdministratorAsync();

        await this.SalesPermissionService.CheckRequiredPermissionAsync(storeAdministrator,
            SalesPermissions.ManagePromotion);

        await this._promotionService.InsertAsync(dto);

        return new ApiResponse<object>();
    }

    [HttpPost("save")]
    public async Task<ApiResponse<object>> SaveCouponAsync([FromBody] StorePromotionDto dto)
    {
        var storeAdministrator = await this.StoreAuthService.GetRequiredStoreAdministratorAsync();

        await this.SalesPermissionService.CheckRequiredPermissionAsync(storeAdministrator,
            SalesPermissions.ManagePromotion);

        if (string.IsNullOrWhiteSpace(dto.Id))
            await this._promotionService.InsertAsync(dto);
        else
        {
            await this._promotionService.UpdateAsync(dto);
        }

        return new ApiResponse<object>();
    }

    [HttpPost("update-rules")]
    public async Task<ApiResponse<object>> UpdateRulesAsync([FromBody] StorePromotionDto dto)
    {
        var storeAdministrator = await this.StoreAuthService.GetRequiredStoreAdministratorAsync();

        await this.SalesPermissionService.CheckRequiredPermissionAsync(storeAdministrator,
            SalesPermissions.ManagePromotion);

        await this._promotionService.UpdateRulesAsync(dto);

        return new ApiResponse<object>();
    }

    [HttpPost("update-status")]
    public async Task<ApiResponse<object>> UpdateStatusAsync([FromBody] UpdatePromotionStatusInput dto)
    {
        var storeAdministrator = await this.StoreAuthService.GetRequiredStoreAdministratorAsync();

        await this.SalesPermissionService.CheckRequiredPermissionAsync(storeAdministrator,
            SalesPermissions.ManagePromotion);

        await this._promotionService.UpdateStatusAsync(dto);

        return new ApiResponse<object>();
    }
}