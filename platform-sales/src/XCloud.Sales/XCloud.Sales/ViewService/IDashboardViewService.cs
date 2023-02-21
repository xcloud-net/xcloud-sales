using XCloud.Sales.Application;
using XCloud.Sales.Service.AfterSale;
using XCloud.Sales.Service.Catalog;
using XCloud.Sales.Service.Coupons;
using XCloud.Sales.Service.Orders;
using XCloud.Sales.Service.Promotion;
using XCloud.Sales.Service.Report;
using XCloud.Sales.Service.Users;

namespace XCloud.Sales.ViewService;

public interface IDashboardViewService : ISalesAppService
{
    Task<DashboardCountView> CounterAsync(CachePolicy cachePolicy);
}

public class DashboardViewService : SalesAppService, IDashboardViewService
{
    private readonly ICouponService _couponService;
    private readonly IPromotionService _promotionService;
    private readonly IGoodsService _goodsService;
    private readonly IOrderService _orderService;
    private readonly IAfterSaleService _afterSaleService;
    private readonly IUserReportService _userReportService;
    private readonly IUserBalanceService _userBalanceService;
    private readonly IUserPointService _userPointService;
    private readonly IPrepaidCardService _prepaidCardService;
    private readonly IBrandService _brandService;
    private readonly ICategoryService _categoryService;
    private readonly ITagService _tagService;

    public DashboardViewService(ICouponService couponService, IPromotionService promotionService,
        IGoodsService goodsService, IOrderService orderService,
        IAfterSaleService afterSaleService, IUserReportService userReportService,
        IUserBalanceService userBalanceService, IUserPointService userPointService,
        IPrepaidCardService prepaidCardService,
        IBrandService brandService, ICategoryService categoryService, ITagService tagService)
    {
        this._brandService = brandService;
        this._categoryService = categoryService;
        _tagService = tagService;
        this._prepaidCardService = prepaidCardService;
        this._userReportService = userReportService;
        this._userBalanceService = userBalanceService;
        this._userPointService = userPointService;
        this._afterSaleService = afterSaleService;
        this._couponService = couponService;
        this._promotionService = promotionService;
        this._goodsService = goodsService;
        this._orderService = orderService;
    }

    private async Task<DashboardCountView> CounterAsync()
    {
        var dto = new DashboardCountView
        {
            Coupon = await this._couponService.ActiveCouponCountAsync(),
            Promotion = await this._promotionService.ActivePromotionCountAsync(),
            Goods = await this._goodsService.CountAsync(),
            Orders = await this._orderService.QueryPendingCountAsync(new QueryPendingCountInput()),
            AfterSale = await this._afterSaleService.QueryPendingCountAsync(new QueryAftersalePendingCountInput()),
            Balance = await this._userBalanceService.CountAllBalanceAsync(),
            PrepaidCard = await this._prepaidCardService.QueryUnusedAmountTotalAsync(),
            ActiveUser = await this._userReportService.TodayActiveUserCountAsync(),
            Brand = await this._brandService.QueryCountAsync(),
            Category = await this._categoryService.QueryCountAsync(),
            Tag = await this._tagService.QueryCountAsync()
        };

        return dto;
    }

    public async Task<DashboardCountView> CounterAsync(CachePolicy cachePolicy)
    {
        if (cachePolicy == null)
            throw new ArgumentNullException(nameof(cachePolicy));

        var key = $"mall.manage.dashboard.counter.data";
        var option = new CacheOption<DashboardCountView>(key, TimeSpan.FromMinutes(10));

        var data = await this.CacheProvider.ExecuteWithPolicyAsync(
            this.CounterAsync,
            option,
            cachePolicy);

        data = data ?? new DashboardCountView();

        return data;
    }
}