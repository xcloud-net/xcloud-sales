using Microsoft.AspNetCore.Mvc;
using XCloud.Core.Cache;
using XCloud.Core.Dto;
using XCloud.Sales.Service.Authentication;
using XCloud.Sales.Service.Report;
using XCloud.Sales.ViewService;

namespace XCloud.Sales.Mall.Api.Controller.Admin;

[StoreAuditLog]
[Route("api/mall-admin/report")]
public class ReportController : ShopBaseController
{
    private readonly IOrderReportService _orderReportService;
    private readonly IGoodsReportService _goodsReportService;
    private readonly IUserReportService _userReportService;
    private readonly IActivityReportService _activityReportService;
    private readonly IAfterSalesReportService _afterSalesReportService;
    private readonly IDashboardViewService _dashboardViewService;

    public ReportController(
        IDashboardViewService dashboardViewService,
        IAfterSalesReportService afterSalesReportService,
        IActivityReportService activityReportService,
        IUserReportService userReportService,
        IGoodsReportService goodsReportService,
        IOrderReportService orderReportService)
    {
        this._dashboardViewService = dashboardViewService;
        this._afterSalesReportService = afterSalesReportService;
        this._activityReportService = activityReportService;
        this._userReportService = userReportService;
        this._goodsReportService = goodsReportService;
        this._orderReportService = orderReportService;
    }

    [HttpPost("dashboard-counter")]
    public async Task<ApiResponse<DashboardCountView>> DashBoardCounterAsync()
    {
        var storeAdministrator = await this.StoreAuthService.GetRequiredStoreAdministratorAsync();

        await this.SalesPermissionService.CheckRequiredPermissionAsync(storeAdministrator,
            SalesPermissions.ManageReport);

        var response = await this._dashboardViewService.CounterAsync(new CachePolicy() { Cache = true });
        
        return new ApiResponse<DashboardCountView>(response);
    }

    [HttpPost("top-aftersales-users")]
    public async Task<ApiResponse<TopAfterSaleMallUsersResponse[]>> TopAfterSalesMallUsersAsync(
        [FromBody] TopAfterSaleMallUsersInput dto)
    {
        var storeAdministrator = await this.StoreAuthService.GetRequiredStoreAdministratorAsync();

        await this.SalesPermissionService.CheckRequiredPermissionAsync(storeAdministrator,
            SalesPermissions.ManageReport);

        dto.EndTime ??= this.Clock.Now;
        dto.StartTime ??= dto.EndTime.Value.AddMonths(-6);

        var response = await this._afterSalesReportService.TopAfterSalesMallUsersAsync(dto);

        return new ApiResponse<TopAfterSaleMallUsersResponse[]>(response);
    }

    [HttpPost("user-activity-by-geo-location")]
    public async Task<ApiResponse<UserActivityGroupByGeoLocationResponse[]>> GroupByGeoLocationAsync(
        [FromBody] UserActivityGroupByGeoLocationInput dto)
    {
        var storeAdministrator = await this.StoreAuthService.GetRequiredStoreAdministratorAsync();

        await this.SalesPermissionService.CheckRequiredPermissionAsync(storeAdministrator,
            SalesPermissions.ManageReport);

        dto.EndTime ??= this.Clock.Now;
        dto.StartTime ??= dto.EndTime.Value.AddMonths(-1);

        var response = await this._activityReportService.GroupByGeoLocationAsync(dto);

        return new ApiResponse<UserActivityGroupByGeoLocationResponse[]>(response);
    }

    [HttpPost("user-activity-by-hour")]
    public async Task<ApiResponse<UserActivityGroupByHourResponse[]>> UserActivityGroupByHourAsync(
        [FromBody] UserActivityGroupByHourInput dto)
    {
        var storeAdministrator = await this.StoreAuthService.GetRequiredStoreAdministratorAsync();

        await this.SalesPermissionService.CheckRequiredPermissionAsync(storeAdministrator,
            SalesPermissions.ManageReport);

        dto.EndTime ??= this.Clock.Now;
        dto.StartTime ??= dto.EndTime.Value.AddMonths(-1);

        var response = await this._userReportService.UserActivityGroupByHourAsync(dto);

        return new ApiResponse<UserActivityGroupByHourResponse[]>(response);
    }

    [HttpPost("grouped-keywords")]
    public async Task<ApiResponse<QuerySearchKeywordsReportResponse[]>> QuerySearchKeywordsReportAsync(
        [FromBody] QuerySearchKeywordsReportInput dto)
    {
        var storeAdministrator = await this.StoreAuthService.GetRequiredStoreAdministratorAsync();

        await this.SalesPermissionService.CheckRequiredPermissionAsync(storeAdministrator,
            SalesPermissions.ManageReport);

        dto.EndTime ??= this.Clock.Now;
        dto.StartTime ??= dto.EndTime.Value.AddDays(-7);

        var response = await this._goodsReportService.QuerySearchKeywordsReportAsync(dto);

        return new ApiResponse<QuerySearchKeywordsReportResponse[]>(response);
    }

    [HttpPost("top-visited-goods")]
    public async Task<ApiResponse<QueryGoodsVisitReportResponse[]>> TopVisitedGoodsAsync(
        [FromBody] QueryGoodsVisitReportInput dto)
    {
        var storeAdministrator = await this.StoreAuthService.GetRequiredStoreAdministratorAsync();

        await this.SalesPermissionService.CheckRequiredPermissionAsync(storeAdministrator,
            SalesPermissions.ManageReport);

        dto.EndTime = this.Clock.Now;
        dto.StartTime = dto.EndTime.Value.AddDays(-30);

        var response = await this._goodsReportService.TopVisitedGoodsAsync(dto);

        return new ApiResponse<QueryGoodsVisitReportResponse[]>(response);
    }

    [HttpPost("top-customers")]
    public async Task<ApiResponse<TopCustomersList[]>> TopSellersAsync([FromBody] QueryTopCustomerInput dto)
    {
        var storeAdministrator = await this.StoreAuthService.GetRequiredStoreAdministratorAsync();

        await this.SalesPermissionService.CheckRequiredPermissionAsync(storeAdministrator,
            SalesPermissions.ManageReport);

        dto.EndTime ??= this.Clock.Now;
        dto.StartTime ??= dto.EndTime.Value.AddMonths(-1);

        var data = await this._orderReportService.TopCustomersAsync(dto);

        return new ApiResponse<TopCustomersList[]>(data);
    }

    [HttpPost("top-sellers")]
    public async Task<ApiResponse<TopSellersList[]>> TopSellersAsync([FromBody] QueryTopSellerInput dto)
    {
        var storeAdministrator = await this.StoreAuthService.GetRequiredStoreAdministratorAsync();

        await this.SalesPermissionService.CheckRequiredPermissionAsync(storeAdministrator,
            SalesPermissions.ManageReport);

        dto.EndTime ??= this.Clock.Now;
        dto.StartTime ??= dto.EndTime.Value.AddMonths(-1);

        var data = await this._orderReportService.TopSellersAsync(dto);

        return new ApiResponse<TopSellersList[]>(data);
    }

    [HttpPost("top-brands")]
    public async Task<ApiResponse<TopBrandList[]>> TopBrandsAsync([FromBody] QueryTopBrandListInput dto)
    {
        var storeAdministrator = await this.StoreAuthService.GetRequiredStoreAdministratorAsync();

        await this.SalesPermissionService.CheckRequiredPermissionAsync(storeAdministrator,
            SalesPermissions.ManageReport);

        dto.EndTime ??= this.Clock.Now;
        dto.StartTime ??= dto.EndTime.Value.AddMonths(-1);

        var data = await this._orderReportService.TopBrandListsAsync(dto);

        return new ApiResponse<TopBrandList[]>(data);
    }

    [HttpPost("top-category")]
    public async Task<ApiResponse<TopCategoryList[]>> TopCategoriesAsync([FromBody] QueryTopCategoryListInput dto)
    {
        var storeAdministrator = await this.StoreAuthService.GetRequiredStoreAdministratorAsync();

        await this.SalesPermissionService.CheckRequiredPermissionAsync(storeAdministrator,
            SalesPermissions.ManageReport);

        dto.EndTime ??= this.Clock.Now;
        dto.StartTime ??= dto.EndTime.Value.AddMonths(-1);

        var data = await this._orderReportService.TopCategoryListsAsync(dto);

        return new ApiResponse<TopCategoryList[]>(data);
    }

    [HttpPost("top-skus")]
    public async Task<ApiResponse<TopSkuList[]>> TopSkusAsync([FromBody] QueryTopSkuListInput dto)
    {
        var storeAdministrator = await this.StoreAuthService.GetRequiredStoreAdministratorAsync();

        await this.SalesPermissionService.CheckRequiredPermissionAsync(storeAdministrator,
            SalesPermissions.ManageReport);

        dto.EndTime ??= this.Clock.Now;
        dto.StartTime ??= dto.EndTime.Value.AddMonths(-1);

        var data = await this._orderReportService.TopSkuListsAsync(dto);

        return new ApiResponse<TopSkuList[]>(data);
    }

    [HttpPost("order-sum-by-date")]
    public async Task<ApiResponse<OrderSumByDateResponse[]>> OrderSumByDateAsync([FromBody] OrderSumByDateInput dto)
    {
        var storeAdministrator = await this.StoreAuthService.GetRequiredStoreAdministratorAsync();

        await this.SalesPermissionService.CheckRequiredPermissionAsync(storeAdministrator,
            SalesPermissions.ManageReport);

        dto.EndTime ??= this.Clock.Now;
        dto.StartTime ??= dto.EndTime.Value.AddMonths(-1);
        dto.MaxCount ??= 100;

        var response = await this._orderReportService.OrderSumByDateAsync(dto);

        for (var time = dto.StartTime.Value; time < dto.EndTime.Value; time = time.AddDays(1))
        {
            if (response.Any(x => x.Date.Date == time.Date))
                continue;
            response = response.Append(new OrderSumByDateResponse() { Date = time, Total = 0, Amount = 0 })
                .ToArray();
        }

        response = response.OrderBy(x => x.Date).ToArray();

        return new ApiResponse<OrderSumByDateResponse[]>(response);
    }
}