using XCloud.Platform.Application.Common.Service.Settings;
using XCloud.Sales.Application;

namespace XCloud.Sales.Service.Configuration;

public interface IMallSettingService : ISalesAppService
{
    string MallSettingsKey { get; }
    string HomeBlocksKey { get; }

    Task<MallSettingsDto> GetCachedMallSettingsAsync();
    Task SaveMallSettingsAsync(MallSettingsDto dto);

    Task SaveHomeBlocksAsync(HomeBlocksDto dto);
    Task<HomeBlocksDto> GetCachedHomeBlocksAsync();
}

public class MallSettingService : SalesAppService, IMallSettingService
{
    private readonly ISysSettingsService _sysSettingsService;

    public MallSettingService(ISysSettingsService sysSettingsService)
    {
        _sysSettingsService = sysSettingsService;
    }

    public string MallSettingsKey => "mall-settings";
    public string HomeBlocksKey => "mall-home-blocks";

    public async Task SaveMallSettingsAsync(MallSettingsDto dto)
    {
        if (dto == null)
            throw new ArgumentNullException(nameof(dto));

        await this._sysSettingsService.SaveObjectAsync(this.MallSettingsKey, dto);
    }

    public async Task<MallSettingsDto> GetCachedMallSettingsAsync()
    {
        var settings = await this._sysSettingsService.GetObjectOrDefaultAsync<MallSettingsDto>(this.MallSettingsKey);
        if (settings != null)
            return settings;

        return new MallSettingsDto();
    }

    public async Task SaveHomeBlocksAsync(HomeBlocksDto dto)
    {
        if (dto == null)
            throw new ArgumentNullException(nameof(dto));

        await this._sysSettingsService.SaveObjectAsync(this.HomeBlocksKey, dto);
    }

    public async Task<HomeBlocksDto> GetCachedHomeBlocksAsync()
    {
        var settings = await this._sysSettingsService.GetObjectOrDefaultAsync<HomeBlocksDto>(this.HomeBlocksKey);
        if (settings != null)
            return settings;

        return new HomeBlocksDto();
    }
}