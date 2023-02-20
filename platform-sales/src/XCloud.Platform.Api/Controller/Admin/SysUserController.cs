using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using XCloud.AspNetMvc.ModelBinder.JsonModel;
using XCloud.Core.Dto;
using XCloud.Platform.Auth.Application.Admin;
using XCloud.Platform.Core.Domain.User;
using XCloud.Platform.Framework.Controller;
using XCloud.Platform.Member.Application.Service.User;

namespace XCloud.Platform.Api.Controller.Admin;

[Route("/api/sys/user")]
public class SysUserController : PlatformBaseController, IAdminController
{
    private readonly IUserProfileService _userProfileService;
    private readonly IUserAccountService _userAccountService;
    private readonly IUserMobileService _userMobileService;
    public SysUserController(IUserProfileService userProfileService,
        IUserAccountService userAccountService,
        IUserMobileService userMobileService)
    {
        this._userProfileService = userProfileService;
        this._userAccountService = userAccountService;
        this._userMobileService = userMobileService;
    }

    [HttpPost("query-user-account")]
    public async Task<ApiResponse<SysUserDto>> QueryUserAccount([FromBody] QueryUserAccountInput dto)
    {
        var admin = await this.GetRequiredAuthedAdminAsync();

        var account = await this._userAccountService.QueryUserAccountAsync(dto.AccountIdentity);

        return new ApiResponse<SysUserDto>(account);
    }

    [HttpPost("search-top-users")]
    public async Task<ApiResponse<SysUserDto[]>> SearchTopUsers([FromBody] QueryUserDto dto)
    {
        var admin = await this.GetRequiredAuthedAdminAsync();

        var users = await this._userProfileService.QueryTopUsersAsync(dto);

        return new ApiResponse<SysUserDto[]>(users);
    }

    [HttpPost("users-by-ids")]
    public async Task<ApiResponse<SysUserDto[]>> UsersDtoByIds([FromBody] string[] ids)
    {
        var admin = await this.GetRequiredAuthedAdminAsync();

        var data = await this._userProfileService.QueryUserProfileByIdsAsync(ids);

        await this._userProfileService.AttachDataAsync(data, new AttachUserDataInput() { Mobile = true });

        return new ApiResponse<SysUserDto[]>(data);
    }

    /// <summary>
    /// 查询user
    /// </summary>
    [HttpPost("paging")]
    public async Task<PagedResponse<SysUserDto>> QueryUserPagination([JsonData] QueryUserDto dto)
    {
        dto.PageSize = 20;

        var admin = await this.GetRequiredAuthedAdminAsync();

        var data = await this._userProfileService.QueryUserProfilePaginationAsync(dto);

        data.Items = await this._userProfileService.AttachDataAsync(data.Items.ToArray(), new AttachUserDataInput() { Mobile = true });

        return data;
    }

    [HttpPost("set-pwd")]
    public async Task<ApiResponse<object>> SetPasswordAsync([FromBody] ResetUserPasswordInput dto)
    {
        var admin = await this.GetRequiredAuthedAdminAsync();

        await this._userAccountService.SetPasswordAsync(dto.Id, dto.Password);

        return new ApiResponse<object>();
    }

    [HttpPost("set-mobile")]
    public async Task<ApiResponse<SysUserIdentity>> SetMobileAsync([FromBody] SetUserMobileInput dto)
    {
        var admin = await this.GetRequiredAuthedAdminAsync();

        var res = await this._userMobileService.SetUserMobilePhoneAsync(dto.Id, dto.Mobile);

        if (res.IsSuccess())
        {
            await this._userMobileService.ConfirmMobileAsync(res.Data.Id);
        }

        return res;
    }

    [HttpPost("remove-mobiles")]
    public async Task<ApiResponse<object>> RemoveMobilesAsync([FromBody] IdDto dto)
    {
        var admin = await this.GetRequiredAuthedAdminAsync();

        await this._userMobileService.RemoveUserMobilesAsync(dto.Id);

        return new ApiResponse<object>();
    }

    [HttpPost("create")]
    public async Task<ApiResponse<SysUser>> CreateUserAsync([FromBody] IdentityNameDto dto)
    {
        var admin = await this.GetRequiredAuthedAdminAsync();

        var res = await this._userAccountService.CreateUserAccountAsync(dto);

        return res;
    }

    [HttpPost("update-status")]
    public async Task<ApiResponse<object>> UpdateStatusAsync([FromBody] UpdateUserStatusDto dto)
    {
        var admin = await this.GetRequiredAuthedAdminAsync();

        await this._userAccountService.UpdateUserStatusAsync(dto);

        return new ApiResponse<object>();
    }
}