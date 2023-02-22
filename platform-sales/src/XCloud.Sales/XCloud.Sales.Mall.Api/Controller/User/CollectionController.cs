using Microsoft.AspNetCore.Mvc;
using XCloud.Core.Dto;
using XCloud.Sales.Service.Authentication;
using XCloud.Sales.Service.Catalog;
using XCloud.Sales.Service.Users;

namespace XCloud.Sales.Mall.Api.Controller.User;

[Route("api/mall/collection")]
public class GoodsCollectionController : ShopBaseController
{
    private readonly IGoodsCollectionService _collectionService;
    private readonly IGoodsService _goodsService;
    private readonly IGoodsPriceService _goodsPriceService;
    private readonly IUserGradeService _userGradeService;

    public GoodsCollectionController(IGoodsCollectionService collectionService, IGoodsService goodsService,
        IGoodsPriceService goodsPriceService, IUserGradeService userGradeService)
    {
        this._collectionService = collectionService;
        this._goodsService = goodsService;
        this._goodsPriceService = goodsPriceService;
        this._userGradeService = userGradeService;
    }

    [HttpPost("by-id")]
    public async Task<ApiResponse<GoodsCollectionDto>> QueryByIdAsync([FromBody] IdDto dto)
    {
        var collection = await this._collectionService.QueryByIdAsync(dto.Id);
        if (collection == null)
            throw new EntityNotFoundException(nameof(collection));

        await this._collectionService.AttachCollectionItemsDataAsync(new[] { collection });

        var goods = collection.Items
            .SelectMany(x => new[] { x.Goods, x.GoodsSpecCombination?.Goods })
            .WhereNotNull().ToArray();

        var combinations = collection.Items
            .Select(x => x.GoodsSpecCombination)
            .WhereNotNull().ToArray();

        await this._goodsService.AttachDataAsync(goods, new AttachGoodsDataInput() { Images = true });

        var storeUserOrNull = await this.StoreAuthService.GetStoreUserOrNullAsync();
        if (storeUserOrNull != null)
        {
            var grade = await this._userGradeService.GetGradeByUserIdAsync(storeUserOrNull.Id);
            if (grade != null)
            {
                await this._goodsPriceService.AttachGradePriceAsync(combinations, grade.Id);
            }
        }
        else
        {
            var settings = await this.MallSettingService.GetCachedMallSettingsAsync();
            if (settings.HidePriceForGuest)
            {
                foreach (var m in goods)
                {
                    m.HidePrice();
                }

                foreach (var m in combinations)
                {
                    m.HidePrice();
                }
            }
        }

        return new ApiResponse<GoodsCollectionDto>(collection);
    }

    [HttpPost("top")]
    public async Task<ApiResponse<GoodsCollectionDto[]>> AllAsync()
    {
        var data = await this._collectionService.QueryAllAsync();

        if (!data.Any())
            return new ApiResponse<GoodsCollectionDto[]>();

        data = await this._collectionService.AttachCollectionItemsDataAsync(data);

        var goods = data.SelectMany(x => x.Items)
            .SelectMany(x => new[] { x.Goods, x.GoodsSpecCombination?.Goods })
            .WhereNotNull().ToArray();

        var combinations = data.SelectMany(x => x.Items)
            .Select(x => x.GoodsSpecCombination)
            .WhereNotNull().ToArray();

        await this._goodsService.AttachDataAsync(goods, new AttachGoodsDataInput() { Images = true });

        var storeUserOrNull = await this.StoreAuthService.GetStoreUserOrNullAsync();
        if (storeUserOrNull != null)
        {
            var grade = await this._userGradeService.GetGradeByUserIdAsync(storeUserOrNull.Id);
            if (grade != null)
            {
                await this._goodsPriceService.AttachGradePriceAsync(combinations, grade.Id);
            }
        }
        else
        {
            var settings = await this.MallSettingService.GetCachedMallSettingsAsync();
            if (settings.HidePriceForGuest)
            {
                foreach (var m in goods)
                {
                    m.HidePrice();
                }

                foreach (var m in combinations)
                {
                    m.HidePrice();
                }
            }
        }

        return new ApiResponse<GoodsCollectionDto[]>(data);
    }
}