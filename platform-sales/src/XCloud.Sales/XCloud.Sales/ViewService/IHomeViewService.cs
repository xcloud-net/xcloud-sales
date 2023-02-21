using XCloud.Sales.Application;
using XCloud.Sales.Service.Catalog;
using XCloud.Sales.Service.Common;
using XCloud.Sales.Service.Configuration;
using XCloud.Sales.Service.Coupons;
using XCloud.Sales.Service.Promotion;

namespace XCloud.Sales.ViewService;

public interface IHomeViewService : ISalesAppService
{
    Task<HomePageDto> QueryHomePageDtoAsync(CachePolicy cachePolicyOption);
}

public class HomeViewService : SalesAppService, IHomeViewService
{
    private readonly IGoodsSearchService _goodsSearchService;
    private readonly IGoodsService _goodsService;
    private readonly IMallSettingService _mallSettingService;
    private readonly IPagesService _pagesService;
    private readonly ICategoryService _categoryService;
    private readonly IPromotionService _promotionService;
    private readonly ICouponService _couponService;

    public HomeViewService(IGoodsSearchService goodsSearchService, IGoodsService goodsService,
        IMallSettingService mallSettingService,
        IPagesService pagesService,
        IPromotionService promotionService,
        ICouponService couponService,
        ICategoryService categoryService)
    {
        this._promotionService = promotionService;
        this._couponService = couponService;
        this._categoryService = categoryService;
        this._pagesService = pagesService;
        this._mallSettingService = mallSettingService;
        this._goodsSearchService = goodsSearchService;
        this._goodsService = goodsService;
    }

    private async Task<HomePageDto> QueryHomePageDtoAsync()
    {
        var dto = new HomePageDto
        {
            Coupons = await this.QueryCouponsAsync(),
            Promotions = await this.QueryPromotionsAsync(),
            Topic = await this.GetTopicAsync(),
            HotGoods = await this.ListHotGoodsAsync(),
            NewGoods = await this.ListNewGoodsAsync(),
            CategoryAndGoods = await this.QueryHomePageCategoryAndGoodsDtoAsync()
        };

        dto = await this.HandleHomePageDataAsync(dto);

        return dto;
    }

    public async Task<HomePageDto> QueryHomePageDtoAsync(CachePolicy cachePolicyOption)
    {
        if (cachePolicyOption == null)
            throw new ArgumentNullException(nameof(cachePolicyOption));

        var key = $"mall.home.page.data";
        var option = new CacheOption<HomePageDto>(key, TimeSpan.FromMinutes(10));

        var data = await this.CacheProvider.ExecuteWithPolicyAsync(
            this.QueryHomePageDtoAsync,
            option,
            cachePolicyOption);

        data = data ?? new HomePageDto();

        return data;
    }

    private async Task<CouponDto[]> QueryCouponsAsync()
    {
        var dto = new QueryCouponPagingInput
        {
            Page = 1,
            PageSize = 5,
            IsDeleted = false,
            AvailableFor = this.Clock.Now,
            SkipCalculateTotalCount = true
        };

        var response = await this._couponService.QueryPagingAsync(dto);

        return response.Items.ToArray();
    }

    private async Task<StorePromotionDto[]> QueryPromotionsAsync()
    {
        var dto = new QueryPromotionPagingInput()
        {
            Page = 1,
            PageSize = 5,
            IsDeleted = false,
            AvailableFor = this.Clock.Now,
            SkipCalculateTotalCount = true
        };

        var response = await this._promotionService.QueryPagingAsync(dto);

        return response.Items.ToArray();
    }

    private async Task<HomePageDto> HandleHomePageDataAsync(HomePageDto dto)
    {
        var allGoods = dto.AllGoods().ToArray();

        foreach (var m in allGoods)
        {
            m.HideDetail();
        }

        var option = new AttachGoodsDataInput()
        {
            Images = true,
            Combination = true
        };

        await this._goodsService.AttachDataAsync(allGoods, option);

        return dto;
    }

    private async Task<HomePageCategoryAndGoodsDto[]> QueryHomePageCategoryAndGoodsDtoAsync()
    {
        var categoryAndGoods = new List<HomePageCategoryAndGoodsDto>();
        var settings = await this._mallSettingService.GetCachedMallSettingsAsync();
        if (!string.IsNullOrWhiteSpace(settings.HomePageCategorySeoNames))
        {
            var seoNames = settings.HomePageCategorySeoNames.Split(',', '|', ';').Select(x => x.Trim()).ToArray();
            var index = 0;
            foreach (var name in seoNames)
            {
                if (string.IsNullOrWhiteSpace(name))
                    continue;
                var category = await this._categoryService.QueryBySeoNameAsync(name);
                if (category == null)
                    continue;
                var goodsList = await this._goodsSearchService.SearchGoodsV2Async(new SearchProductsInput()
                {
                    SkipCalculateTotalCount = true,
                    IsDeleted = false,
                    IsPublished = true,
                    CategoryId = category.Id,
                    Page = 1,
                    PageSize = 10
                });
                var item = new HomePageCategoryAndGoodsDto()
                {
                    Category = category,
                    Goods = goodsList.Items.ToArray(),
                    Order = ++index
                };
                categoryAndGoods.Add(item);
            }
        }

        return categoryAndGoods.ToArray();
    }

    private async Task<PagesDto> GetTopicAsync()
    {
        var dto = new QueryPagesInput
        {
            SkipCalculateTotalCount = true,
            IsPublished = true,
            Page = 1,
            PageSize = 1
        };

        var res = await this._pagesService.QueryPagingAsync(dto);

        if (!res.IsNotEmpty)
            return null;

        await this._pagesService.AttachDataAsync(res.Items.ToArray(),
            new AttachPageDataInput() { CoverImage = true });

        return res.Items.FirstOrDefault();
    }

    private async Task<GoodsDto[]> ListHotGoodsAsync()
    {
        var dto = new SearchProductsInput
        {
            PageSize = 10,
            SkipCalculateTotalCount = true,
            IsPublished = true,
            IsHot = true,
            IsDeleted = false
        };

        var response = await this._goodsSearchService.SearchGoodsV2Async(dto);

        return response.Items.ToArray();
    }

    private async Task<GoodsDto[]> ListNewGoodsAsync()
    {
        var dto = new SearchProductsInput
        {
            PageSize = 10,
            SkipCalculateTotalCount = true,
            IsPublished = true,
            IsNew = true,
            IsDeleted = false
        };

        var response = await this._goodsSearchService.SearchGoodsV2Async(dto);

        return response.Items.ToArray();
    }
}