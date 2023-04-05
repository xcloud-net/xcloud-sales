using Microsoft.AspNetCore.Mvc;
using XCloud.Core.Dto;
using XCloud.Sales.Data.Domain.Catalog;
using XCloud.Sales.Service.Authentication;
using XCloud.Sales.Service.Catalog;
using XCloud.Sales.Service.Search;
using XCloud.Sales.Service.Stores;
using XCloud.Sales.Service.Users;

namespace XCloud.Sales.Mall.Api.Controller.Admin;

[StoreAuditLog]
[Route("api/mall-admin/combination")]
public class GoodsSpecCombinationController : ShopBaseController
{
    private readonly IGoodsService _goodsService;
    private readonly ISpecCombinationService _specCombinationService;
    private readonly ISpecCombinationPriceService _specCombinationPriceService;
    private readonly IGoodsSearchService _goodsSearchService;
    private readonly IGoodsStockService _goodsStockService;
    private readonly IUserGradeService _userGradeService;
    private readonly IStoreGoodsMappingService _storeGoodsMappingService;
    private readonly IGradeGoodsPriceService _gradeGoodsPriceService;

    public GoodsSpecCombinationController(
        IUserGradeService userGradeService,
        IGoodsStockService goodsStockService,
        IGoodsSearchService goodsSearchService,
        ISpecCombinationPriceService specCombinationPriceService,
        IGoodsService goodsService,
        ISpecCombinationService specCombinationService,
        IStoreGoodsMappingService storeGoodsMappingService, IGradeGoodsPriceService gradeGoodsPriceService)
    {
        this._userGradeService = userGradeService;
        this._goodsStockService = goodsStockService;
        this._goodsSearchService = goodsSearchService;
        this._specCombinationPriceService = specCombinationPriceService;
        this._goodsService = goodsService;
        this._specCombinationService = specCombinationService;
        _storeGoodsMappingService = storeGoodsMappingService;
        _gradeGoodsPriceService = gradeGoodsPriceService;
    }

    [HttpPost("query-combination-for-selection")]
    public async Task<ApiResponse<GoodsSpecCombinationDto[]>> QueryCombinationForSelectionAsync(
        [FromBody] QueryCombinationForSelectionInput dto)
    {
        dto.Take = 30;

        var storeAdministrator = await this.StoreAuthService.GetRequiredStoreAdministratorAsync();

        await this.SalesPermissionService.CheckRequiredPermissionAsync(storeAdministrator,
            SalesPermissions.ManageCatalog);

        var combinations = await this._specCombinationService.QueryGoodsCombinationForSelectionAsync(dto);

        return new ApiResponse<GoodsSpecCombinationDto[]>(combinations);
    }

    [HttpPost("query-paging")]
    public async Task<PagedResponse<GoodsSpecCombinationDto>> QueryGoodsCombinationPaginationAsync(
        [FromBody] QueryGoodsCombinationInput dto)
    {
        dto.IsDeleted = null;

        var storeAdministrator = await this.StoreAuthService.GetRequiredStoreAdministratorAsync();

        await this.SalesPermissionService.CheckRequiredPermissionAsync(storeAdministrator,
            SalesPermissions.ManageCatalog);

        var response = await this._goodsSearchService.QueryGoodsCombinationPaginationAsync(dto);

        if (!response.IsNotEmpty)
            return response;

        var combinations = response.Items.ToArray();
        await this._specCombinationService.AttachDataAsync(combinations,
            new GoodsCombinationAttachDataInput()
            {
                DeserializeSpecCombinationJson = true,
                SpecCombinationDetail = true,
                CalculateSpecCombinationErrors = true,
                GradePrices = true,
                Stores = true,
                Images = true
            });

        //todo remove
        var goods = response.Items.Select(x => x.Goods).Where(x => x != null).ToArray();
        await this._goodsService.AttachDataAsync(goods, new AttachGoodsDataInput() { Images = true });

        return response;
    }

    [HttpPost("update-spec-combination")]
    public async Task<ApiResponse<object>> SetSpecCombinationAsync([FromBody] GoodsSpecCombinationDto dto)
    {
        var storeAdministrator = await this.StoreAuthService.GetRequiredStoreAdministratorAsync();

        await this.SalesPermissionService.CheckRequiredPermissionAsync(storeAdministrator,
            SalesPermissions.ManageCatalog);

        await this._specCombinationService.SetSpecCombinationAsync(dto);

        return new ApiResponse<object>();
    }

    [HttpPost("update-stock")]
    public async Task<ApiResponse<object>> UpdateStockAsync([FromBody] GoodsSpecCombinationDto dto)
    {
        var storeAdministrator = await this.StoreAuthService.GetRequiredStoreAdministratorAsync();

        await this.SalesPermissionService.CheckRequiredPermissionAsync(storeAdministrator,
            SalesPermissions.ManageCatalog);

        var combination = await this._specCombinationService.QueryByIdAsync(dto.Id);
        if (combination == null)
            throw new EntityNotFoundException(nameof(combination));

        await this._goodsStockService.SetCombinationStockAsync(combination.Id, dto.StockQuantity);

        await this.SalesEventBusService.NotifyRefreshGoodsInfo(new IdDto<int>(combination.GoodsId));

        return new ApiResponse<object>();
    }

    [HttpPost("update-price")]
    public async Task<ApiResponse<object>> UpdatePriceAsync([FromBody] GoodsSpecCombinationDto dto)
    {
        var storeAdministrator = await this.StoreAuthService.GetRequiredStoreAdministratorAsync();

        await this.SalesPermissionService.CheckRequiredPermissionAsync(storeAdministrator,
            SalesPermissions.ManageCatalog);

        var combination = await this._specCombinationService.QueryByIdAsync(dto.Id);
        if (combination == null)
            throw new EntityNotFoundException(nameof(combination));

        if (combination.CostPrice != dto.CostPrice)
            await this._specCombinationPriceService.UpdateCombinationCostPriceAsync(new UpdateGoodsCostPriceDto()
                { Id = combination.Id, CostPrice = dto.CostPrice });

        if (combination.Price != dto.Price)
            await this._specCombinationPriceService.UpdateCombinationPriceAsync(new UpdateGoodsPriceDto()
                { Id = combination.Id, Price = dto.Price });

        if (dto.GradePriceToSave != null)
            await this._gradeGoodsPriceService.SaveGradePriceAsync(combination.Id, dto.GradePriceToSave);

        await this.SalesEventBusService.NotifyRefreshGoodsInfo(new IdDto<int>(combination.GoodsId));

        return new ApiResponse<object>();
    }

    [HttpPost("update-status")]
    public async Task<ApiResponse<object>> UpdateGoodsSpecCombinationStatusAsync(
        [FromBody] UpdateGoodsSpecCombinationStatusInput dto)
    {
        var storeAdministrator = await this.StoreAuthService.GetRequiredStoreAdministratorAsync();

        await this.SalesPermissionService.CheckRequiredPermissionAsync(storeAdministrator,
            SalesPermissions.ManageCatalog);

        var entity = await this._specCombinationService.QueryByIdAsync(dto.Id);
        if (entity == null)
            throw new EntityNotFoundException(nameof(UpdateGoodsSpecCombinationStatusAsync));

        await this._specCombinationService.UpdateStatusAsync(dto);

        await this.SalesEventBusService.NotifyRefreshGoodsInfo(new IdDto<int>(entity.GoodsId));

        return new ApiResponse<object>();
    }

    [HttpPost("list-by-goodsid")]
    public async Task<ApiResponse<GoodsSpecCombinationDto[]>> ListCombinationsAsync([FromBody] IdDto<int> dto)
    {
        var storeAdministrator = await this.StoreAuthService.GetRequiredStoreAdministratorAsync();

        await this.SalesPermissionService.CheckRequiredPermissionAsync(storeAdministrator,
            SalesPermissions.ManageCatalog);

        var goods = await this._goodsService.QueryByIdAsync(dto.Id);
        if (goods == null)
            throw new EntityNotFoundException(nameof(goods));

        var combinations = await this._specCombinationService.QueryByGoodsIdAsync(dto.Id);

        var combinationDtos = combinations
            .Select(x => this.ObjectMapper.Map<GoodsSpecCombination, GoodsSpecCombinationDto>(x))
            .ToArray();

        await this._specCombinationService.AttachDataAsync(combinationDtos,
            new GoodsCombinationAttachDataInput()
            {
                DeserializeSpecCombinationJson = true,
                SpecCombinationDetail = true,
                CalculateSpecCombinationErrors = true,
                GradePrices = true
            });

        return new ApiResponse<GoodsSpecCombinationDto[]>(combinationDtos);
    }

    /// <summary>
    /// for scanner
    /// </summary>
    [HttpPost("by-sku")]
    public async Task<ApiResponse<GoodsSpecCombinationDto>> BySkuAsync([FromBody] CombinationBySkuInput dto)
    {
        var storeAdministrator = await this.StoreAuthService.GetRequiredStoreAdministratorAsync();

        await this.SalesPermissionService.CheckRequiredPermissionAsync(storeAdministrator,
            SalesPermissions.ManageCatalog);

        var combination = await this._specCombinationService.QueryBySkuAsync(dto.Sku);
        if (combination == null)
            throw new EntityNotFoundException(nameof(BySkuAsync));

        if (dto.UserId != null)
        {
            var grade = await this._userGradeService.GetGradeByUserIdAsync(dto.UserId.Value);
            if (grade != null)
            {
                await this._gradeGoodsPriceService.AttachGradePriceAsync(new[] { combination }, grade.Id);
            }
        }

        return new ApiResponse<GoodsSpecCombinationDto>(combination);
    }

    [HttpPost("save-v1")]
    public virtual async Task<ApiResponse<GoodsSpecCombination>> SaveCombinationsAsync(
        [FromBody] GoodsSpecCombinationDto dto)
    {
        var storeAdministrator = await this.StoreAuthService.GetRequiredStoreAdministratorAsync();

        await this.SalesPermissionService.CheckRequiredPermissionAsync(storeAdministrator,
            SalesPermissions.ManageCatalog);

        var goods = await this._goodsService.QueryByIdAsync(dto.GoodsId);
        if (goods == null)
            throw new EntityNotFoundException(nameof(SaveCombinationsAsync));

        if (dto.Id > 0)
        {
            var res = await this._specCombinationService.UpdateAsync(dto);

            await this.SalesEventBusService.NotifyRefreshGoodsInfo(new IdDto<int>(goods.Id));

            return new ApiResponse<GoodsSpecCombination>(res);
        }
        else
        {
            var res = await this._specCombinationService.InsertAsync(dto);

            await this.SalesEventBusService.NotifyRefreshGoodsInfo(new IdDto<int>(goods.Id));

            return new ApiResponse<GoodsSpecCombination>(res);
        }
    }

    [HttpPost("store-mapping-list")]
    public async Task<ApiResponse<StoreDto[]>> StoreMappingList([FromBody] IdDto<int> dto)
    {
        var storeAdministrator = await this.StoreAuthService.GetRequiredStoreAdministratorAsync();

        await this.SalesPermissionService.CheckRequiredPermissionAsync(storeAdministrator,
            SalesPermissions.ManageCatalog);

        var combination = await _specCombinationService.QueryByIdAsync(dto.Id);
        if (combination == null)
            throw new EntityNotFoundException(nameof(StoreMappingList));

        var combinationDto = this.ObjectMapper.Map<GoodsSpecCombination, GoodsSpecCombinationDto>(combination);

        await this._specCombinationService.AttachDataAsync(new[] { combinationDto },
            new GoodsCombinationAttachDataInput() { Stores = true });

        return new ApiResponse<StoreDto[]>(combinationDto.Stores);
    }

    [HttpPost("save-store-mapping")]
    public virtual async Task<ApiResponse<object>> SaveStoreMapping([FromBody] SaveGoodsStoreMappingDto dto)
    {
        var storeAdministrator = await this.StoreAuthService.GetRequiredStoreAdministratorAsync();

        await this.SalesPermissionService.CheckRequiredPermissionAsync(storeAdministrator,
            SalesPermissions.ManageCatalog);

        var combination = await _specCombinationService.QueryByIdAsync(dto.GoodsCombinationId);

        if (combination == null)
            throw new EntityNotFoundException(nameof(SaveStoreMapping));

        await this._storeGoodsMappingService.SaveGoodsStoreMappingAsync(dto);

        await this.SalesEventBusService.NotifyRefreshGoodsInfo(new IdDto<int>(combination.GoodsId));

        return new ApiResponse<object>();
    }
}