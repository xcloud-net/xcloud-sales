using Microsoft.AspNetCore.Mvc;
using XCloud.Core.Application;
using XCloud.Core.Dto;
using XCloud.Sales.Data.Domain.Catalog;
using XCloud.Sales.Services.Catalog;

namespace XCloud.Sales.Mall.Api.Controllers.Admin;

[StoreAuditLog]
[Route("api/mall-admin/grade")]
public class GradePriceController : ShopBaseController
{
    private readonly IGoodsService _goodsService;
    private readonly IGoodsPriceService _goodsPriceService;
    private readonly IGoodsSpecCombinationService _goodsSpecCombinationService;

    public GradePriceController(IGoodsService goodsService,
        IGoodsPriceService goodsPriceService,
        IGoodsSpecCombinationService goodsSpecCombinationService)
    {
        this._goodsService = goodsService;
        this._goodsPriceService = goodsPriceService;
        _goodsSpecCombinationService = goodsSpecCombinationService;
    }

    [HttpPost("delete-grade-price")]
    public virtual async Task<ApiResponse<object>> DeleteGradePriceOffsetAsync([FromBody] IdDto dto)
    {
        var storeAdministrator = await this.StoreAuthService.GetRequiredStoreAdministratorAsync();

        await this.SalesPermissionService.CheckRequiredPermissionAsync(storeAdministrator,
            SalesPermissions.ManageCatalog);

        await this._goodsPriceService.DeleteGradePriceAsync(dto.Id);

        return new ApiResponse<object>();
    }

    [HttpPost("save-goods-grade-price")]
    public virtual async Task<ApiResponse<object>> SaveGradePriceAsync([FromBody] GoodsGradePriceDto offset)
    {
        var storeAdministrator = await this.StoreAuthService.GetRequiredStoreAdministratorAsync();

        await this.SalesPermissionService.CheckRequiredPermissionAsync(storeAdministrator,
            SalesPermissions.ManageCatalog);

        await this._goodsPriceService.SetGradePriceAsync(offset);

        return new ApiResponse<object>();
    }

    [HttpPost("list-combination-grade-price")]
    public async Task<ApiResponse<GoodsGradePriceDto[]>> ListCombinationGradePriceAsync([FromBody] IdDto<int> dto)
    {
        var storeAdministrator = await this.StoreAuthService.GetRequiredStoreAdministratorAsync();

        await this.SalesPermissionService.CheckRequiredPermissionAsync(storeAdministrator,
            SalesPermissions.ManageCatalog);

        throw new NotImplementedException();
    }

    [Obsolete]
    [HttpPost("list-goods-grade-price")]
    public async Task<ApiResponse<GoodsGradePriceDto[]>> ListGoodsGradePriceOffset([FromBody] IdDto<int> dto)
    {
        var storeAdministrator = await this.StoreAuthService.GetRequiredStoreAdministratorAsync();

        await this.SalesPermissionService.CheckRequiredPermissionAsync(storeAdministrator,
            SalesPermissions.ManageCatalog);

        var goods = await _goodsService.QueryByIdAsync(dto.Id);

        if (goods == null)
            throw new EntityNotFoundException(nameof(goods));

        var combinations = await this._goodsSpecCombinationService.QueryByGoodsIdAsync(goods.Id);
        var allCombinations = this.ObjectMapper.MapArray<GoodsSpecCombination, GoodsSpecCombinationDto>(combinations);

        await this._goodsSpecCombinationService.AttachDataAsync(allCombinations,
            new GoodsCombinationAttachDataInput() { GradePrices = true });

        var response = allCombinations.SelectMany(x => x.AllGradePrices).ToArray();

        await this._goodsPriceService.AttachDataAsync(response,
            new GoodsGradePriceAttachDataInput() { GoodsInfo = true });

        response = response
            .OrderByDescending(x => x.GoodsCombinationId)
            .ThenByDescending(x => x.CreationTime).ToArray();

        return new ApiResponse<GoodsGradePriceDto[]>(response);
    }
}