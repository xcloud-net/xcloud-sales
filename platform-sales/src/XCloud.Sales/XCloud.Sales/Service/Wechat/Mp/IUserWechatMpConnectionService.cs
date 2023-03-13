using Microsoft.Extensions.Options;
using XCloud.Platform.Application.Member.Service.User;
using XCloud.Platform.Connection.WeChat.Settings;
using XCloud.Sales.Application;
using XCloud.Sales.Clients.Platform;
using XCloud.Sales.Service.Users;

namespace XCloud.Sales.Service.Wechat.Mp;

public interface IUserWechatMpConnectionService : ISalesAppService
{
    Task<WechatProfileDto> QueryUserWechatMpConnectionAsync(int userId);
}

public class UserWechatMpConnectionService : SalesAppService, IUserWechatMpConnectionService
{
    private readonly IOptions<WechatMpOption> _wechatMpOption;
    private readonly PlatformInternalService _platformInternalService;
    private readonly IUserService _userService;

    public UserWechatMpConnectionService(PlatformInternalService platformInternalService, IUserService userService,
        IOptions<WechatMpOption> wechatMpOption)
    {
        _platformInternalService = platformInternalService;
        _userService = userService;
        _wechatMpOption = wechatMpOption;
    }

    public async Task<WechatProfileDto> QueryUserWechatMpConnectionAsync(int userId)
    {
        var user = await this._userService.GetUserByIdAsync(userId);
        
        if (user == null)
            throw new EntityNotFoundException(nameof(user));
        
        if (string.IsNullOrWhiteSpace(user.GlobalUserId))
            throw new UserFriendlyException("global user id is required");

        var connection = await this._platformInternalService.QueryUserConnectionAsync(new QueryUserConnectionRequest()
        {
            UserId = user.GlobalUserId,
            Platform = ThirdPartyPlatforms.WxMp,
            AppId = this._wechatMpOption.Value.AppId,
        });

        if (connection == null)
            return null;

        return new WechatProfileDto()
        {
            OpenId = connection.OpenId
        };
    }
}