using Microsoft.AspNetCore.Mvc;
using XCloud.Sales.ViewService;
using XCloud.Core.Cache;
using XCloud.Core.Dto;
using XCloud.Platform.Common.Application.Service.Settings;
using XCloud.Sales.Service.Authentication;
using XCloud.Sales.Service.Configuration;

namespace XCloud.Sales.Mall.Api.Controllers.Admin;

[StoreAuditLog]
[Route("api/mall-admin/setting")]
public class SettingController : ShopBaseController
{
    private readonly IMallSettingService _mallSettingService;
    private readonly IHomeViewService _homeViewService;
    private readonly ICategoryViewService _categoryViewService;
    private readonly IDashboardViewService _dashboardViewService;
    private readonly ISearchViewService _searchViewService;
    private readonly ISysSettingsService _sysSettingsService;

    public SettingController(IDashboardViewService dashboardViewService,
        ISearchViewService searchViewService,
        IHomeViewService homeViewService,
        ICategoryViewService categoryViewService,
        IMallSettingService mallSettingService,
        ISysSettingsService sysSettingsService)
    {
        this._searchViewService = searchViewService;
        this._dashboardViewService = dashboardViewService;
        this._homeViewService = homeViewService;
        this._categoryViewService = categoryViewService;
        this._mallSettingService = mallSettingService;
        _sysSettingsService = sysSettingsService;
    }

    [HttpPost("refresh-view-cache")]
    public async Task<ApiResponse<object>> RefreshViewCacheAsync([FromBody] RefreshViewCacheInput dto)
    {
        var storeAdministrator = await this.StoreAuthService.GetRequiredStoreAdministratorAsync();

        await this.SalesPermissionService.CheckRequiredPermissionAsync(storeAdministrator,
            SalesPermissions.ManageSettings);

        if (dto.MallSettings)
        {
            await this._sysSettingsService.GetSettingValueByNameAsync(
                this._mallSettingService.MallSettingsKey,
                new CachePolicy() { Refresh = true });
        }

        if (dto.Home)
        {
            await this._sysSettingsService.GetSettingValueByNameAsync(
                this._mallSettingService.HomeBlocksKey,
                new CachePolicy() { Refresh = true });
            await this._homeViewService.QueryHomePageDtoAsync(new CachePolicy() { Refresh = true });
        }

        if (dto.RootCategory)
        {
            await this._categoryViewService.QueryCategoryPageDataAsync(new CachePolicy() { Refresh = true });
        }

        if (dto.Dashboard)
        {
            await this._dashboardViewService.CounterAsync(new CachePolicy() { Refresh = true });
        }

        if (dto.Search)
        {
            await this._searchViewService.QuerySearchViewAsync(new CachePolicy() { Refresh = true });
        }

        return new ApiResponse<object>();
    }

    [HttpPost("mall-settings")]
    public async Task<ApiResponse<MallSettingsDto>> GetMallSettingsAsync()
    {
        var storeAdministrator = await this.StoreAuthService.GetRequiredStoreAdministratorAsync();

        await this.SalesPermissionService.CheckRequiredPermissionAsync(storeAdministrator,
            SalesPermissions.ManageSettings);

        var settings = await this._mallSettingService.GetCachedMallSettingsAsync();

        return new ApiResponse<MallSettingsDto>(settings);
    }

    [HttpPost("save-mall-settings")]
    public async Task<ApiResponse<object>> SaveMallSettingsAsync([FromBody] MallSettingsDto dto)
    {
        var storeAdministrator = await this.StoreAuthService.GetRequiredStoreAdministratorAsync();

        await this.SalesPermissionService.CheckRequiredPermissionAsync(storeAdministrator,
            SalesPermissions.ManageSettings);

        await this._mallSettingService.SaveMallSettingsAsync(dto);

        return new ApiResponse<object>();
    }

    [HttpPost("home-blocks")]
    public async Task<ApiResponse<HomeBlocksDto>> GetHomeBlocksAsync()
    {
        var storeAdministrator = await this.StoreAuthService.GetRequiredStoreAdministratorAsync();

        await this.SalesPermissionService.CheckRequiredPermissionAsync(storeAdministrator,
            SalesPermissions.ManageSettings);

        var blocks = await this._mallSettingService.GetCachedHomeBlocksAsync();

        return new ApiResponse<HomeBlocksDto>(blocks);
    }

    [HttpPost("save-home-blocks")]
    public async Task<ApiResponse<object>> SaveHomeBlocksAsync([FromBody] HomeBlocksDto dto)
    {
        var storeAdministrator = await this.StoreAuthService.GetRequiredStoreAdministratorAsync();

        await this.SalesPermissionService.CheckRequiredPermissionAsync(storeAdministrator,
            SalesPermissions.ManageSettings);

        await this._mallSettingService.SaveHomeBlocksAsync(dto);

        return new ApiResponse<object>();
    }
}