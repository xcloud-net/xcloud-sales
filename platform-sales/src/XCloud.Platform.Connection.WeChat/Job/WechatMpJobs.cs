using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Volo.Abp.DependencyInjection;
using XCloud.Platform.Connection.WeChat.Services;

namespace XCloud.Platform.Connection.WeChat.Job;

public class WechatMpJobs : ITransientDependency
{
    private readonly IWxMpService _wxMpService;
    private readonly ILogger _logger;
    public WechatMpJobs(IWxMpService wxMpService, ILogger<WechatMpJobs> logger)
    {
        this._wxMpService = wxMpService;
        this._logger = logger;
    }

    public async Task RefreshClientAccessTokenAsync()
    {
        try
        {
            await this._wxMpService.GetOrRefreshClientAccessTokenAsync();
            this._logger.LogInformation("refresh wx-mp client access token");
        }
        catch (Exception e)
        {
            this._logger.LogError(message: e.Message, exception: e);
        }
    }
}