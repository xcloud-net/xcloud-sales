using Microsoft.AspNetCore.Mvc;
using XCloud.Core.Dto;
using XCloud.Sales.Service.Authentication;
using XCloud.Sales.Service.Promotion;

namespace XCloud.Sales.Mall.Api.Controllers;

[Route("api/mall/promotion")]
public class PromotionController : ShopBaseController
{
    private readonly IPromotionService _promotionService;

    public PromotionController(IPromotionService promotionService)
    {
        this._promotionService = promotionService;
    }
    
    /// <summary>
    /// for place order selection
    /// </summary>
    /// <param name="dto"></param>
    /// <returns></returns>
    [HttpPost("paging")]
    public async Task<PagedResponse<StorePromotionDto>> QueryCouponPagingAsync([FromBody] QueryPromotionPagingInput dto )
    {
        var storeAdministrator = await this.StoreAuthService.GetRequiredStoreAdministratorAsync();
        
        dto.IsDeleted = false;
        dto.IsActive = true;
        dto.AvailableFor = this.Clock.Now;
        dto.SkipCalculateTotalCount = true;
        
        var response = await this._promotionService.QueryPagingAsync(dto);

        return response;
    }
}