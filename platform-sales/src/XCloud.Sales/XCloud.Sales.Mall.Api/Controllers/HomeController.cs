using Microsoft.AspNetCore.Mvc;
using XCloud.Sales.Services.Catalog;
using XCloud.Sales.Services.Coupons;
using XCloud.Sales.Services.Users;
using XCloud.Sales.ViewService;
using XCloud.Core.Cache;
using XCloud.Core.Dto;
using XCloud.Core.Helper;

namespace XCloud.Sales.Mall.Api.Controllers;

[Route("api/mall/home")]
public class HomeController : ShopBaseController
{
    private readonly IHomeViewService _homeViewService;
    private readonly IUserGradeService _userGradeService;
    private readonly IGoodsPriceService _goodsPriceService;
    private readonly ICouponService _couponService;

    public HomeController(IHomeViewService homeViewService,
        IUserGradeService userGradeService,
        ICouponService couponService,
        IGoodsPriceService goodsPriceService)
    {
        this._couponService = couponService;
        this._userGradeService = userGradeService;
        this._goodsPriceService = goodsPriceService;
        this._homeViewService = homeViewService;
    }

    [HttpPost("view")]
    public async Task<ApiResponse<HomePageDto>> HomePageAsync()
    {
        var data = await this._homeViewService.QueryHomePageDtoAsync(new CachePolicy() { Cache = true });

        var allGoods = data.AllGoods().ToArray();

        var loginUserOrNull = await this.StoreAuthService.GetStoreUserOrNullAsync();
        if (loginUserOrNull == null)
        {
            var settings = await this.MallSettingService.GetCachedMallSettingsAsync();
            if (settings.HidePriceForGuest)
                foreach (var m in allGoods)
                {
                    m.HidePrice();
                }
        }
        else
        {
            var grade = await this._userGradeService.GetGradeByUserIdAsync(loginUserOrNull.Id);
            if (grade != null)
            {
                var combinations = allGoods.SelectMany(x => x.GoodsSpecCombinations).WhereNotNull().ToArray();
                await this._goodsPriceService.AttachGradePriceAsync(combinations, grade.Id);
            }
        }

        if (ValidateHelper.IsNotEmptyCollection(data.Coupons) && loginUserOrNull != null)
        {
            await this._couponService.AttachDataAsync(data.Coupons,
                new AttachCouponDataInput() { IssuedToUserId = loginUserOrNull.Id });
        }

        return new ApiResponse<HomePageDto>(data);
    }
}