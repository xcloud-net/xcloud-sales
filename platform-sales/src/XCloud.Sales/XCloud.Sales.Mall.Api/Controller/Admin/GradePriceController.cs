using Microsoft.AspNetCore.Mvc;
using XCloud.Application.Mapper;
using XCloud.Core.Application;
using XCloud.Core.Dto;
using XCloud.Sales.Data.Domain.Catalog;
using XCloud.Sales.Service.Authentication;
using XCloud.Sales.Service.Catalog;
using XCloud.Sales.Service.Users;

namespace XCloud.Sales.Mall.Api.Controller.Admin;

[StoreAuditLog]
[Route("api/mall-admin/grade")]
public class GradePriceController : ShopBaseController
{
    private readonly IGoodsService _goodsService;
    private readonly ISpecCombinationPriceService _specCombinationPriceService;
    private readonly ISpecCombinationService _specCombinationService;
    private readonly IGradeGoodsPriceService _gradeGoodsPriceService;

    public GradePriceController(IGoodsService goodsService,
        ISpecCombinationPriceService specCombinationPriceService,
        ISpecCombinationService specCombinationService, IGradeGoodsPriceService gradeGoodsPriceService)
    {
        this._goodsService = goodsService;
        this._specCombinationPriceService = specCombinationPriceService;
        _specCombinationService = specCombinationService;
        _gradeGoodsPriceService = gradeGoodsPriceService;
    }

    [HttpPost("delete-grade-price")]
    public virtual async Task<ApiResponse<object>> DeleteGradePriceOffsetAsync([FromBody] IdDto dto)
    {
        var storeAdministrator = await this.StoreAuthService.GetRequiredStoreAdministratorAsync();

        await this.SalesPermissionService.CheckRequiredPermissionAsync(storeAdministrator,
            SalesPermissions.ManageCatalog);

        await this._gradeGoodsPriceService.DeleteGradePriceAsync(dto.Id);

        return new ApiResponse<object>();
    }

    [HttpPost("save-goods-grade-price")]
    public virtual async Task<ApiResponse<object>> SaveGradePriceAsync([FromBody] GoodsGradePriceDto offset)
    {
        var storeAdministrator = await this.StoreAuthService.GetRequiredStoreAdministratorAsync();

        await this.SalesPermissionService.CheckRequiredPermissionAsync(storeAdministrator,
            SalesPermissions.ManageCatalog);

        await this._gradeGoodsPriceService.SetGradePriceAsync(offset);

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

        var combinations = await this._specCombinationService.QueryByGoodsIdAsync(goods.Id);
        var allCombinations = this.ObjectMapper.MapArray<GoodsSpecCombination, GoodsSpecCombinationDto>(combinations);

        await this._specCombinationService.AttachDataAsync(allCombinations,
            new GoodsCombinationAttachDataInput() { GradePrices = true });

        var response = allCombinations.SelectMany(x => x.AllGradePrices).ToArray();

        await this._gradeGoodsPriceService.AttachDataAsync(response,
            new GoodsGradePriceAttachDataInput() { GoodsInfo = true });

        response = response
            .OrderByDescending(x => x.GoodsCombinationId)
            .ThenByDescending(x => x.CreationTime).ToArray();

        return new ApiResponse<GoodsGradePriceDto[]>(response);
    }
}