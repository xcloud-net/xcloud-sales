using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using XCloud.Core.Dto;
using XCloud.Platform.Application.Member.Service.User;
using XCloud.Platform.Auth.Application.User;
using XCloud.Platform.Framework.Controller;

namespace XCloud.Platform.Api.Controller.User;

[Route("/api/platform/user/mobile")]
public class UserMobilePhoneController : PlatformBaseController, IUserController
{
    private readonly IUserMobileService userMobileService;

    public UserMobilePhoneController(
        IUserMobileService userMobileService)
    {
        this.userMobileService = userMobileService;
    }

    /// <summary>
    /// 读取用户绑定的手机号
    /// </summary>
    /// <returns></returns>
    [HttpPost("mine")]
    public async Task<ApiResponse<object>> GetUserPhone()
    {
        var loginuser = await this.GetRequiredAuthedUserAsync();

        var data = await this.userMobileService.GetUserMobilePhonesAsync(loginuser.UserId);

        var phone = data.FirstOrDefault();

        if (phone == null)
            return new ApiResponse<object>().SetError("not bind");

        return new ApiResponse<object>().SetData(phone.MobilePhone);
    }
}