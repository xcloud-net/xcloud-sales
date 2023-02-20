using Microsoft.AspNetCore.Mvc;
using XCloud.Sales.Data.Domain.Logging;
using XCloud.Sales.Services.Catalog;
using XCloud.Sales.Services.Logging;
using XCloud.Sales.Services.Report;
using XCloud.Sales.Services.Users;
using XCloud.Sales.ViewService;
using XCloud.Core.Cache;
using XCloud.Core.Dto;

namespace XCloud.Sales.Mall.Api.Controllers;

/// <summary>
/// 商品接口
/// </summary>
[Route("api/mall/goods")]
public class GoodsController : ShopBaseController
{
    private readonly IGoodsService _goodsService;
    private readonly IGoodsPriceService _goodsPriceService;
    private readonly IUserGradeService _userGradeService;
    private readonly IGoodsReportService _goodsReportService;
    private readonly IGoodsViewService _goodsViewService;
    private readonly IFavoritesService _favoritesService;
    private readonly ICombinationViewService _combinationViewService;
    private readonly IGoodsSpecCombinationService _goodsSpecCombinationService;

    /// <summary>
    /// 构造器
    /// </summary>
    public GoodsController(
        IFavoritesService favoritesService,
        IGoodsViewService goodsViewService,
        IGoodsReportService goodsReportService,
        IGoodsService goodsService,
        IGoodsPriceService goodsPriceService,
        IUserGradeService userGradeService,
        ICombinationViewService combinationViewService,
        IGoodsSpecCombinationService goodsSpecCombinationService)
    {
        this._favoritesService = favoritesService;
        this._goodsViewService = goodsViewService;
        this._goodsReportService = goodsReportService;
        this._goodsService = goodsService;
        this._goodsPriceService = goodsPriceService;
        this._userGradeService = userGradeService;
        _combinationViewService = combinationViewService;
        _goodsSpecCombinationService = goodsSpecCombinationService;
    }

    [HttpPost("combination-price-history")]
    public async Task<ApiResponse<PriceHistoryResponse[]>> QueryCombinationPriceHistoryAsync(
        [FromBody] QueryCombinationPriceHistoryInput dto)
    {
        var storeUser = await this.StoreAuthService.GetRequiredStoreUserAsync();

        var now = this.Clock.Now;

        if (dto.LastDays != null)
        {
            dto.EndTime = now;
            dto.StartTime = dto.EndTime.Value.AddDays(-dto.LastDays.Value);
        }

        dto.EndTime ??= now;
        dto.StartTime ??= now.AddDays(-30);

        var data = await this._goodsReportService.QueryCombinationPriceHistoryAsync(dto);

        var response = new List<PriceHistoryResponse>();

        //first price
        var previousPrice = data.OrderBy(x => x.CreationTime).Select(x => x.PreviousPrice).FirstOrDefault();
        for (var m = dto.StartTime.Value; m <= dto.EndTime.Value; m = m.AddDays(1))
        {
            var date = m.Date;
            var price = default(decimal);

            //find the last price offset at that day
            var offsetOrNull = data.Where(x => x.CreationTime.Date == date).MaxBy(x => x.CreationTime);

            if (offsetOrNull == null)
                price = previousPrice;
            else
                price = offsetOrNull.Price;

            response.Add(new PriceHistoryResponse()
            {
                Date = date,
                Price = price,
                NoData = offsetOrNull == null
            });
            //continue

            previousPrice = price;
        }

        return new ApiResponse<PriceHistoryResponse[]>(response.ToArray());
    }

    [HttpPost("query-spec-group")]
    public async Task<ApiResponse<SpecGroupSelectResponse>> QuerySpecGroupMenuAsync(
        [FromBody] QueryCombinationInput dto)
    {
        var response = await this._combinationViewService.QuerySpecGroupMenuAsync(dto);

        return new ApiResponse<SpecGroupSelectResponse>(response);
    }

    [HttpPost("query-combination")]
    public async Task<ApiResponse<GoodsSpecCombinationDto[]>> QueryAvailableCombinationsAsync(
        [FromBody] QueryCombinationInput dto)
    {
        var matchedCombinations = await this._combinationViewService.QueryAvailableCombinationsAsync(dto);

        await this._goodsSpecCombinationService.AttachDataAsync(matchedCombinations,
            new GoodsCombinationAttachDataInput()
            {
                DeserializeSpecCombinationJson = true,
                SpecCombinationDetail = true
            });

        var loginUser = await this.StoreAuthService.GetStoreUserOrNullAsync();

        if (loginUser != null)
        {
            var grade = await this._userGradeService.GetGradeByUserIdAsync(loginUser.Id);
            if (grade != null)
            {
                matchedCombinations = (
                    await this._goodsPriceService.AttachGradePriceAsync(
                        matchedCombinations.ToArray(),
                        grade.Id)
                ).ToArray();
            }
        }
        else
        {
            var settings = await this.MallSettingService.GetCachedMallSettingsAsync();
            if (settings.HidePriceForGuest)
                foreach (var m in matchedCombinations)
                    m.HidePrice();
        }

        return new ApiResponse<GoodsSpecCombinationDto[]>(matchedCombinations);
    }

    [HttpPost("multiple-by-ids")]
    public async Task<ApiResponse<GoodsDto[]>> MultipleByIds([FromBody] int[] dto)
    {
        var data = await this._goodsService.QueryByIdsAsync(dto);

        data = await this._goodsService.AttachDataAsync(data, new AttachGoodsDataInput()
        {
            Images = true,
            Combination = true
        });

        var loginUser = await this.StoreAuthService.GetStoreUserOrNullAsync();
        if (loginUser == null)
        {
            var settings = await this.MallSettingService.GetCachedMallSettingsAsync();

            if (settings.HidePriceForGuest)
                foreach (var m in data)
                    m.HidePrice();
        }
        else
        {
            var grade = await this._userGradeService.GetGradeByUserIdAsync(loginUser.Id);
            if (grade != null)
            {
                var combinations = data.SelectMany(x => x.GoodsSpecCombinations).ToArray();
                await this._goodsPriceService.AttachGradePriceAsync(combinations, grade.Id);
            }
        }

        return new ApiResponse<GoodsDto[]>(data);
    }

    [HttpPost("detail")]
    public async Task<ApiResponse<GoodsDto>> QueryGoodsDetailAsync([FromBody] IdDto<int> dto)
    {
        var goods = await this._goodsViewService.QueryGoodsDetailByIdAsync(dto.Id,
            new CachePolicy() { Cache = true });

        if (goods == null || goods.IsDeleted || !goods.Published)
            return new ApiResponse<GoodsDto>().SetError("goods is deleted");

        var loginUserOrNull = await this.StoreAuthService.GetStoreUserOrNullAsync();

        if (loginUserOrNull != null)
        {
            goods.IsFavorite = await this._favoritesService.CheckIsFavoritesAsync(loginUserOrNull.Id, goods.Id,
                new CachePolicy() { Cache = true });

            await this.EventBusService.NotifyInsertActivityLog(new ActivityLog()
            {
                UserId = loginUserOrNull.Id,
                ActivityLogTypeId = (int)ActivityLogType.VisitGoods,
                SubjectIntId = goods.Id,
                SubjectType = ActivityLogSubjectType.Goods,
                Comment = "查询商品详情"
            });
        }
        else
        {
            var settings = await this.MallSettingService.GetCachedMallSettingsAsync();
            if (settings.HidePriceForGuest)
                goods.HidePrice();

            await this.EventBusService.NotifyInsertActivityLog(new ActivityLog()
            {
                UserId = default,
                ActivityLogTypeId = (int)ActivityLogType.VisitGoods,
                SubjectIntId = goods.Id,
                SubjectType = ActivityLogSubjectType.Goods,
                Comment = "未登录查询商品详情"
            });
        }

        return new ApiResponse<GoodsDto>(goods);
    }

    [HttpPost("detail-by-seoname")]
    public async Task<ApiResponse<GoodsDto>> GetBySeoNameAsync([FromBody] NameDto dto)
    {
        var goodsDto = await _goodsService.QueryBySeoNameAsync(dto.Name);

        if (goodsDto == null || !goodsDto.Published)
            return new ApiResponse<GoodsDto>().SetError("goods is deleted");

        return await this.QueryGoodsDetailAsync(new IdDto<int>(goodsDto.Id));
    }
}