using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using XCloud.Core.Dto;
using XCloud.Core.Helper;
using XCloud.Platform.Shared.Exceptions;
using XCloud.Platform.Auth.Application.User;
using XCloud.Platform.Member.Application.Service.Admin;
using XCloud.Platform.Member.Application.Service.User;

namespace XCloud.Platform.Auth.Application.Admin;

public interface IAdminController : IUserController
{
    //
}

public static class AdminAuthExtension
{
    public static async Task<SysAdminDto> GetRequiredAuthedAdminAsync(this IAdminAuthService adminAuthService,
        SysUserDto currentAuthUser)
    {
        var res = await adminAuthService.GetAuthAdminAsync(currentAuthUser);

        if (res.IsSuccess())
            return res.Data;

        throw new NoLoginException(res.Error.Message);
    }

    public static async Task<SysAdminDto> GetRequiredAuthedAdminAsync<T>(this T controller)
        where T : ControllerBase, IAdminController
    {
        var currentAuthUser = await controller.GetRequiredAuthedUserAsync();

        var adminAuthService = controller.HttpContext.RequestServices.GetRequiredService<IAdminAuthService>();

        return await adminAuthService.GetRequiredAuthedAdminAsync(currentAuthUser);
    }
    
    public static bool IsEmpty(this AdminPermissionRequirement dto)
    {
        IEnumerable<bool> HasRequirements()
        {
            yield return ValidateHelper.IsNotEmptyCollection(dto.Permissions);
            yield return ValidateHelper.IsNotEmptyCollection(dto.Roles);
        }

        return HasRequirements().Any(x => x == true);
    }
}