using Microsoft.AspNetCore.Mvc;
using XCloud.Core.Dto;
using XCloud.Sales.Data.Domain.Catalog;
using XCloud.Core.Helper;
using XCloud.Sales.Service.Authentication;
using XCloud.Sales.Service.Catalog;

namespace XCloud.Sales.Mall.Api.Controllers.Admin;

[StoreAuditLog]
[Route("api/mall-admin/goods")]
public class GoodsController : ShopBaseController
{
    private readonly IGoodsService _goodsService;
    private readonly IGoodsSearchService _goodsSearchService;
    private readonly IGoodsSpecCombinationService _goodsSpecCombinationService;

    public GoodsController(IGoodsService goodsService,
        IGoodsSearchService goodsSearchService, 
        IGoodsSpecCombinationService goodsSpecCombinationService)
    {
        this._goodsService = goodsService;
        this._goodsSearchService = goodsSearchService;
        _goodsSpecCombinationService = goodsSpecCombinationService;
    }

    [HttpPost("multiple-by-ids")]
    public async Task<ApiResponse<GoodsDto[]>> MultipleByIds([FromBody] int[] dto)
    {
        var storeAdministrator = await this.StoreAuthService.GetRequiredStoreAdministratorAsync();

        await this.SalesPermissionService.CheckRequiredPermissionAsync(storeAdministrator,
            SalesPermissions.ManageCatalog);

        var data = await this._goodsService.QueryByIdsAsync(dto);

        data = await this._goodsService.AttachDataAsync(data, new AttachGoodsDataInput()
        {
            Images = true,
        });

        return new ApiResponse<GoodsDto[]>(data);
    }

    [HttpPost("query-goods-for-selection")]
    public async Task<ApiResponse<GoodsDto[]>> QueryGoodsForSelectionAsync([FromBody] SearchProductsInput dto)
    {
        dto.Page = 1;
        dto.PageSize = 30;
        dto.SkipCalculateTotalCount = true;

        var storeAdministrator = await this.StoreAuthService.GetRequiredStoreAdministratorAsync();

        await this.SalesPermissionService.CheckRequiredPermissionAsync(storeAdministrator,
            SalesPermissions.ManageCatalog);

        var goods = await this._goodsSearchService.SearchGoodsV2Async(dto);

        return new ApiResponse<GoodsDto[]>(goods.Items.ToArray());
    }

    [Obsolete]
    [HttpPost("query-combination-for-selection")]
    public async Task<ApiResponse<GoodsSpecCombinationDto[]>> QueryCombinationForSelectionAsync([FromBody] QueryCombinationForSelectionInput dto)
    {
        dto.Take = 30;

        var storeAdministrator = await this.StoreAuthService.GetRequiredStoreAdministratorAsync();

        await this.SalesPermissionService.CheckRequiredPermissionAsync(storeAdministrator,
            SalesPermissions.ManageCatalog);

        var combinations = await this._goodsSpecCombinationService.QueryGoodsCombinationForSelectionAsync(dto);

        return new ApiResponse<GoodsSpecCombinationDto[]>(combinations);
    }

    [HttpPost("set-goods-tags")]
    public virtual async Task<ApiResponse<object>> SetGoodsTagsAsync([FromBody] SetGoodsTagsInput dto)
    {
        var storeAdministrator = await this.StoreAuthService.GetRequiredStoreAdministratorAsync();

        await this.SalesPermissionService.CheckRequiredPermissionAsync(storeAdministrator,
            SalesPermissions.ManageCatalog);

        await this._goodsService.SetTagsAsync(dto);
            
        await this.EventBusService.NotifyRefreshGoodsInfo(new IdDto<int>(dto.Id));

        return new ApiResponse<object>();
    }

    [HttpPost("set-seo-name")]
    public virtual async Task<ApiResponse<Goods>> SetSeoNameAsync([FromBody] SetGoodsSeoNameInput dto)
    {
        var storeAdministrator = await this.StoreAuthService.GetRequiredStoreAdministratorAsync();

        await this.SalesPermissionService.CheckRequiredPermissionAsync(storeAdministrator,
            SalesPermissions.ManageCatalog);

        var res = await this._goodsService.SetSeoNameAsync(dto.GoodsId, dto.SeoName);
            
        await this.EventBusService.NotifyRefreshGoodsInfo(new IdDto<int>(dto.GoodsId));
            
        return res;
    }

    [HttpPost("by-id")]
    public async Task<ApiResponse<GoodsDto>> ById([FromBody] IdDto<int> dto)
    {
        var storeAdministrator = await this.StoreAuthService.GetRequiredStoreAdministratorAsync();

        await this.SalesPermissionService.CheckRequiredPermissionAsync(storeAdministrator,
            SalesPermissions.ManageCatalog);

        var goods = await _goodsService.QueryByIdAsync(dto.Id);

        if (goods == null)
            throw new EntityNotFoundException(nameof(ById));

        var goodsDto = this.ObjectMapper.Map<Goods, GoodsDto>(goods);

        var list = await this._goodsService.AttachDataAsync(new[] { goodsDto }, new AttachGoodsDataInput()
        {
            Brand = true,
            Category = true,
            Images = true,
            Tags = true,
        });

        goodsDto = list.First();

        return new ApiResponse<GoodsDto>(goodsDto);
    }

    [HttpPost("paging")]
    public async Task<PagedResponse<GoodsDto>> QueryGoodsPagingAsync([FromBody] SearchProductsInput dto)
    {
        var storeAdministrator = await this.StoreAuthService.GetRequiredStoreAdministratorAsync();

        await this.SalesPermissionService.CheckRequiredPermissionAsync(storeAdministrator,
            SalesPermissions.ManageCatalog);

        dto.PageSize = 20;
        dto.IsDeleted = null;
        var response = await _goodsSearchService.SearchGoodsV2Async(dto);

        if (ValidateHelper.IsEmptyCollection(response.Items))
            return new PagedResponse<GoodsDto>();

        var option = dto.AttachDataOptions ?? new AttachGoodsDataInput()
        {
            Brand = true,
            Category = true,
            Images = true,
            Combination = true
        };

        await this._goodsService.AttachDataAsync(response.Items.ToArray(), option);

        foreach (var m in response.Items)
        {
            m.HideDetail();
        }

        return response;
    }

    [HttpPost("save")]
    public virtual async Task<ApiResponse<Goods>> Edit([FromBody] Goods model)
    {
        var storeAdministrator = await this.StoreAuthService.GetRequiredStoreAdministratorAsync();

        await this.SalesPermissionService.CheckRequiredPermissionAsync(storeAdministrator,
            SalesPermissions.ManageCatalog);

        if (model.Id > 0)
        {
            var goods = await _goodsService.QueryByIdAsync(model.Id);
            if (goods == null)
                throw new EntityNotFoundException(nameof(goods));

            var res = await this._goodsService.UpdateAsync(model);
                
            await this.EventBusService.NotifyRefreshGoodsInfo(new IdDto<int>(goods.Id));
                
            return res;
        }
        else
        {
            var res = await this._goodsService.InsertAsync(model);
                
            //await this.EventBusService.NotifyRefreshGoodsInfo(new IdDto<int>(res.Data.Id));
                
            return res;
        }
    }

    [HttpPost("update-status")]
    public virtual async Task<ApiResponse<object>> UpdateStatus([FromBody] UpdateGoodsStatusInput dto)
    {
        var storeAdministrator = await this.StoreAuthService.GetRequiredStoreAdministratorAsync();

        await this.SalesPermissionService.CheckRequiredPermissionAsync(storeAdministrator,
            SalesPermissions.ManageCatalog);

        var goods = await _goodsService.QueryByIdAsync(dto.GoodsId);
        if (goods == null)
            throw new EntityNotFoundException(nameof(goods));

        await _goodsService.UpdateStatusAsync(dto);
            
        await this.EventBusService.NotifyRefreshGoodsInfo(new IdDto<int>(goods.Id));

        return new ApiResponse<object>();
    }

    [Obsolete]
    [HttpPost("save-images-v1")]
    public virtual async Task<ApiResponse<object>> SaveImagesV1([FromBody] SaveGoodsImagesInput dto)
    {
        var storeAdministrator = await this.StoreAuthService.GetRequiredStoreAdministratorAsync();

        await this.SalesPermissionService.CheckRequiredPermissionAsync(storeAdministrator,
            SalesPermissions.ManageCatalog);

        var goods = await _goodsService.QueryByIdAsync(dto.Id);
        if (goods == null)
            throw new EntityNotFoundException(nameof(goods));

        await this._goodsService.SaveGoodsImagesV1Async(dto);
            
        await this.EventBusService.NotifyRefreshGoodsInfo(new IdDto<int>(goods.Id));

        return new ApiResponse<object>();
    }
    
    [HttpPost("save-combination-images-v1")]
    public virtual async Task<ApiResponse<object>> SaveCombinationImagesV1([FromBody] SaveGoodsImagesInput dto)
    {
        var storeAdministrator = await this.StoreAuthService.GetRequiredStoreAdministratorAsync();

        await this.SalesPermissionService.CheckRequiredPermissionAsync(storeAdministrator,
            SalesPermissions.ManageCatalog);

        var goods = await _goodsService.QueryByIdAsync(dto.Id);
        if (goods == null)
            throw new EntityNotFoundException(nameof(goods));

        await this._goodsService.SaveGoodsCombinationImagesV1Async(dto);
            
        await this.EventBusService.NotifyRefreshGoodsInfo(new IdDto<int>(goods.Id));

        return new ApiResponse<object>();
    }
}