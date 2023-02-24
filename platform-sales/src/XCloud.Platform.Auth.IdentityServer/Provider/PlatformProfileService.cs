using System;
using System.Linq;
using System.Threading.Tasks;
using IdentityServer4.Models;
using IdentityServer4.Services;
using Microsoft.Extensions.Logging;
using Volo.Abp;
using XCloud.Core.Extension;
using XCloud.Platform.Auth.Authentication;

namespace XCloud.Platform.Auth.IdentityServer.Provider;

/// <summary>
///  Profile就是用户资料，ids 4里面定义了一个IProfileService的接口用来获取用户的一些信息
///  ，主要是为当前的认证上下文绑定claims。我们可以实现IProfileService从外部创建claim扩展到ids4里面。
///  然后返回
/// </summary>
public class PlatformProfileService : IProfileService
{
    private readonly ILogger _logger;

    public PlatformProfileService(ILogger<PlatformProfileService> logger)
    {
        this._logger = logger;
    }

    /// <summary>
    /// 获取用户Claims
    /// 用户请求userinfo endpoint时会触发该方法
    /// http://localhost:5003/connect/userinfo
    /// </summary>
    /// <param name="context"></param>
    /// <returns></returns>
    public async Task GetProfileDataAsync(ProfileDataRequestContext context)
    {
        await Task.CompletedTask;
        try
        {
            //depending on the scope accessing the user data.
            var claims = context.Subject.Claims.ToList();

            //set issued claims to return
            context.IssuedClaims = claims.ToList();
        }
        catch (Exception e)
        {
            this._logger.AddErrorLog(e.Message, e);
        }
    }

    /// <summary>
    /// 判断用户是否可用
    /// Identity Server会确定用户是否有效
    /// </summary>
    /// <param name="context"></param>
    /// <returns></returns>
    public async Task IsActiveAsync(IsActiveContext context)
    {
        try
        {
            var userId = context.Subject?.Claims?.GetSubjectId();
            if (userId == null || userId.Length <= 0)
            {
                throw new UserFriendlyException("subject找不到");
            }

            context.IsActive = true;
            await Task.CompletedTask;
        }
        catch (BusinessException)
        {
            context.IsActive = false;
        }
        catch (Exception e)
        {
            this._logger.AddErrorLog(e.Message, e);
            context.IsActive = false;
        }
    }
}