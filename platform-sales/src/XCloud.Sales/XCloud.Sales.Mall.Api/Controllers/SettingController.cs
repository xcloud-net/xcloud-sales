using Microsoft.AspNetCore.Mvc;
using XCloud.Core.Dto;
using XCloud.Sales.Service.Configuration;

namespace XCloud.Sales.Mall.Api.Controllers;

[Route("api/mall/setting")]
public class SettingController : ShopBaseController
{
    private readonly IMallSettingService _mallSettingService;

    public SettingController(IMallSettingService mallSettingService)
    {
        this._mallSettingService = mallSettingService;
    }

    [HttpPost("mall-settings")]
    public async Task<ApiResponse<MallSettingsDto>> GetMallSettingsAsync()
    {
        var settings = await this._mallSettingService.GetCachedMallSettingsAsync();

        return new ApiResponse<MallSettingsDto>(settings);
    }

    [HttpPost("home-blocks")]
    public async Task<ApiResponse<HomeBlocksDto>> GetHomeBlocksAsync()
    {
        var blocks = await this._mallSettingService.GetCachedHomeBlocksAsync();

        return new ApiResponse<HomeBlocksDto>(blocks);
    }
}