using Microsoft.AspNetCore.Mvc;
using XCloud.Core.Dto;
using XCloud.Sales.Data.Domain.Catalog;
using XCloud.Sales.Service.Authentication;
using XCloud.Sales.Service.Catalog;

namespace XCloud.Sales.Mall.Api.Controller.Admin;

[StoreAuditLog]
[Route("api/mall-admin/collection")]
public class GoodsCollectionController : ShopBaseController
{
    private readonly IGoodsCollectionService _collectionService;
    private readonly IGoodsService _goodsService;
    private readonly IGoodsSpecCombinationService _goodsSpecCombinationService;

    public GoodsCollectionController(IGoodsCollectionService collectionService, 
        IGoodsService goodsService, 
        IGoodsSpecCombinationService goodsSpecCombinationService)
    {
        this._collectionService = collectionService;
        this._goodsService = goodsService;
        _goodsSpecCombinationService = goodsSpecCombinationService;
    }

    [HttpPost("query-combination-for-selection")]
    public async Task<ApiResponse<GoodsSpecCombinationDto[]>> QueryCombinationForSelectionAsync([FromBody] QueryGoodsForCollectionItemSelectionInput dto)
    {
        if (string.IsNullOrWhiteSpace(dto.CollectionId))
            throw new ArgumentNullException(nameof(dto.CollectionId));

        dto.Take = 30;

        var storeAdministrator = await this.StoreAuthService.GetRequiredStoreAdministratorAsync();

        await this.SalesPermissionService.CheckRequiredPermissionAsync(storeAdministrator,
            SalesPermissions.ManageCollection);

        var items = await this._collectionService.QueryItemsByCollectionIdAsync(dto.CollectionId);
        dto.ExcludedCombinationIds = items.Select(x => x.GoodsSpecCombinationId).ToArray();

        var combinations = await this._goodsSpecCombinationService.QueryGoodsCombinationForSelectionAsync(dto);

        return new ApiResponse<GoodsSpecCombinationDto[]>(combinations);
    }

    [HttpPost("all")]
    public async Task<ApiResponse<GoodsCollectionDto[]>> AllAsync()
    {
        var storeAdministrator = await this.StoreAuthService.GetRequiredStoreAdministratorAsync();

        await this.SalesPermissionService.CheckRequiredPermissionAsync(storeAdministrator,
            SalesPermissions.ManageCollection);

        var data = await this._collectionService.QueryAllAsync();

        if (!data.Any())
            return new ApiResponse<GoodsCollectionDto[]>();

        data = await this._collectionService.AttachCollectionItemsDataAsync(data);

        var goods = data.SelectMany(x => x.Items).Select(x => x.Goods).WhereNotNull().ToArray();

        await this._goodsService.AttachDataAsync(goods, new AttachGoodsDataInput() { Images = true });

        return new ApiResponse<GoodsCollectionDto[]>(data);
    }

    [HttpPost("save")]
    public async Task<ApiResponse<GoodsCollection>> SaveAsync([FromBody] GoodsCollectionDto dto)
    {
        var storeAdministrator = await this.StoreAuthService.GetRequiredStoreAdministratorAsync();

        await this.SalesPermissionService.CheckRequiredPermissionAsync(storeAdministrator,
            SalesPermissions.ManageCollection);

        if (string.IsNullOrWhiteSpace(dto.Id))
        {
            return await this._collectionService.InsertAsync(dto);
        }
        else
        {
            return await this._collectionService.UpdateAsync(dto);
        }
    }

    [HttpPost("hide")]
    public async Task<ApiResponse<object>> HideAsync([FromBody] IdDto dto)
    {
        var storeAdministrator = await this.StoreAuthService.GetRequiredStoreAdministratorAsync();

        await this.SalesPermissionService.CheckRequiredPermissionAsync(storeAdministrator,
            SalesPermissions.ManageCollection);

        await this._collectionService.SoftDeleteCollectionAsync(dto.Id);
        return new ApiResponse<object>();
    }

    [HttpPost("add-goods")]
    public async Task<ApiResponse<GoodsCollectionItem>> AddGoodsAsync([FromBody] GoodsCollectionItemDto dto)
    {
        var storeAdministrator = await this.StoreAuthService.GetRequiredStoreAdministratorAsync();

        await this.SalesPermissionService.CheckRequiredPermissionAsync(storeAdministrator,
            SalesPermissions.ManageCollection);

        return await this._collectionService.InsertItemAsync(dto);
    }

    [HttpPost("remove-goods")]
    public async Task<ApiResponse<object>> RemoveGoodsAsync([FromBody] IdDto dto)
    {
        var storeAdministrator = await this.StoreAuthService.GetRequiredStoreAdministratorAsync();

        await this.SalesPermissionService.CheckRequiredPermissionAsync(storeAdministrator,
            SalesPermissions.ManageCollection);

        await this._collectionService.RemoveCollectionItemByIdAsync(dto.Id);

        return new ApiResponse<object>();
    }
}