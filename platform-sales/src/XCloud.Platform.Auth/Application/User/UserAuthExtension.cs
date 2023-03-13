using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using XCloud.Core.Dto;
using XCloud.Platform.Application.Member.Service.User;
using XCloud.Platform.Shared.Exceptions;

namespace XCloud.Platform.Auth.Application.User;

public interface IUserController
{
    //
}

public static class UserAuthExtension
{
    public static async Task<SysUserDto> GetRequiredAuthedUserAsync(this IUserAuthService userAuthService)
    {
        var res = await userAuthService.GetAuthUserAsync();

        if (res.IsSuccess())
            return res.Data;

        if (res.IsNotActive)
            throw new NotActiveException(res.Error.Message);

        if (res.IsDeleted)
            throw new NotActiveException(res.Error.Message);

        throw new NoLoginException(res.Error.Message);
    }

    /// <summary>
    /// 获取登录用户
    /// </summary>
    public static async Task<SysUserDto> GetRequiredAuthedUserAsync<T>(this T controller)
        where T : ControllerBase, IUserController
    {
        var provider = controller.HttpContext.RequestServices;
        var userAuthService = provider.GetRequiredService<IUserAuthService>();

        return await userAuthService.GetRequiredAuthedUserAsync();
    }
}