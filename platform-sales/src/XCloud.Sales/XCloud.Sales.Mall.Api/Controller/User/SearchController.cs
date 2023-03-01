using Microsoft.AspNetCore.Mvc;
using XCloud.Core.Cache;
using XCloud.Core.Dto;
using XCloud.Sales.Data.Domain.Catalog;
using XCloud.Sales.Data.Domain.Logging;
using XCloud.Sales.Service.Authentication;
using XCloud.Sales.Service.Catalog;
using XCloud.Sales.Service.Logging;
using XCloud.Sales.Service.Search;
using XCloud.Sales.Service.Users;
using XCloud.Sales.ViewService;

namespace XCloud.Sales.Mall.Api.Controller.User;

[Route("api/mall/search")]
public class SearchController : ShopBaseController
{
    private readonly ISearchViewService _searchViewService;
    private readonly IBrandService _brandService;
    private readonly ICategoryService _categoryService;
    private readonly ITagService _tagService;
    private readonly IGoodsSearchService _goodsSearchService;
    private readonly IUserGradeService _userGradeService;
    private readonly IGoodsPriceService _goodsPriceService;
    private readonly IGoodsService _goodsService;

    public SearchController(ISearchViewService searchViewService, IBrandService brandService,
        ICategoryService categoryService, ITagService tagService, IGoodsSearchService goodsSearchService,
        IUserGradeService userGradeService, IGoodsPriceService goodsPriceService, IGoodsService goodsService)
    {
        this._searchViewService = searchViewService;
        _brandService = brandService;
        _categoryService = categoryService;
        _tagService = tagService;
        _goodsSearchService = goodsSearchService;
        _userGradeService = userGradeService;
        _goodsPriceService = goodsPriceService;
        _goodsService = goodsService;
    }

    [HttpPost("search-view")]
    public async Task<ApiResponse<SearchView>> SearchViewAsync()
    {
        var response = await this._searchViewService.QuerySearchViewAsync(new CachePolicy() { Cache = true });

        return new ApiResponse<SearchView>(response);
    }

    [HttpPost("search-options")]
    public async Task<ApiResponse<SearchOptionsDto>> SearchOptionsAsync([FromBody] SearchProductsInput dto)
    {
        var options = new SearchOptionsDto();

        if (dto.BrandId != null && dto.BrandId.Value > 0)
        {
            var brand = await this._brandService.QueryByIdAsync(dto.BrandId.Value);
            if (brand != null)
                options.Brand = this.ObjectMapper.Map<Brand, BrandDto>(brand);
        }

        if (dto.CategoryId != null && dto.CategoryId.Value > 0)
        {
            options.Category = await this._categoryService.QueryByIdAsync(dto.CategoryId.Value);
        }

        if (!string.IsNullOrWhiteSpace(dto.TagId))
        {
            options.Tag = await this._tagService.QueryByIdAsync(dto.TagId);
        }

        return new ApiResponse<SearchOptionsDto>(options);
    }

    [NonAction]
    private async Task<PagedResponse<GoodsDto>> HandleSearchResultAsync(PagedResponse<GoodsDto> response,
        SearchProductsInput dto,
        AttachGoodsDataInput option,
        Data.Domain.Users.User loginUserOrNull)
    {
        response.Items = await this._goodsService.AttachDataAsync(response.Items.ToArray(), option);

        foreach (var m in response.Items)
        {
            m.HideDetail();
        }

        if (loginUserOrNull != null)
        {
            var grade = await this._userGradeService.GetGradeByUserIdAsync(loginUserOrNull.Id);
            if (grade != null)
            {
                var combinations = response.Items.SelectMany(x => x.GoodsSpecCombinations).ToArray();

                await this._goodsPriceService.AttachGradePriceAsync(combinations, grade.Id);
            }
        }
        else
        {
            var settings = await this.MallSettingService.GetCachedMallSettingsAsync();
            if (settings.HidePriceForGuest)
                foreach (var m in response.Items)
                {
                    m.HidePrice();
                }
        }

        return response;
    }

    [HttpPost("goods")]
    public async Task<PagedResponse<GoodsDto>> SearchGoodsListAsync([FromBody] SearchProductsInput dto)
    {
        dto.PageSize = 30;
        dto.SkipCalculateTotalCount = true;
        dto.IsPublished = true;
        dto.IsDeleted = false;

        var response = await this._goodsSearchService.SearchGoodsV2Async(dto);

        if (response.IsEmpty)
            return new PagedResponse<GoodsDto>();

        var option = dto.AttachDataOptions ?? new AttachGoodsDataInput()
        {
            Images = true,
            Combination = true
        };

        var loginUser = await this.StoreAuthService.GetStoreUserOrNullAsync();

        response = await this.HandleSearchResultAsync(response, dto, option, loginUser);

        if (dto.Page == 1 && !string.IsNullOrWhiteSpace(dto.Keywords) && loginUser != null)
            await this.EventBusService.NotifyInsertActivityLog(new ActivityLog()
            {
                UserId = loginUser.Id,
                ActivityLogTypeId = (int)ActivityLogType.SearchGoods,
                Value = dto.Keywords.Trim().ToLower(),
                Comment = "keyword-searching",
                Data = string.Empty
            });

        return response;
    }
}