using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Volo.Abp.DependencyInjection;
using XCloud.Platform.Connection.WeChat.Services;
using XCloud.Platform.Core.Application;

namespace XCloud.Platform.Connection.WeChat.Job;

public class WechatMpJobs : PlatformApplicationService, ITransientDependency
{
    private readonly IWxMpService _wxMpService;
    private readonly ILogger _logger;
    private readonly IOptions<WechatMpOption> _options;

    public WechatMpJobs(IWxMpService wxMpService, ILogger<WechatMpJobs> logger, IOptions<WechatMpOption> options)
    {
        this._wxMpService = wxMpService;
        this._logger = logger;
        _options = options;
    }

    public async Task RefreshClientAccessTokenAsync()
    {
        try
        {
            if (string.IsNullOrWhiteSpace(_options.Value.AppId))
                return;

            await this._wxMpService.GetOrRefreshClientAccessTokenAsync();
            this._logger.LogInformation("refresh wx-mp client access token");
        }
        catch (Exception e)
        {
            this._logger.LogError(message: e.Message, exception: e);
        }
    }
}