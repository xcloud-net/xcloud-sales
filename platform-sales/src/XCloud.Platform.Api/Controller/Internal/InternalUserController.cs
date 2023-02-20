using Microsoft.AspNetCore.Mvc;

using System.Threading.Tasks;
using XCloud.Core.Dto;
using XCloud.Platform.Core.Domain.User;
using XCloud.Platform.Framework.Controller;
using XCloud.Platform.Member.Application.Service.User;

namespace XCloud.Platform.Api.Controller.Internal;

[Route("/internal-api/platform/user")]
public class InternalUserController : PlatformBaseController
{
    private readonly IUserProfileService _userProfileService;
    private readonly IExternalConnectService _externalConnectService;
    public InternalUserController(IUserProfileService userProfileService,
        IExternalConnectService externalConnectService)
    {
        this._userProfileService = userProfileService;
        this._externalConnectService = externalConnectService;
    }

    [HttpPost("query-connection")]
    public async Task<ApiResponse<SysExternalConnect>> QueryUserConnectionAsync([FromBody] QueryUserConnectionRequest dto)
    {
        var data = await this._externalConnectService.FindByUserIdAsync(dto.Platform, dto.AppId, dto.UserId);

        if (data == null)
            return new ApiResponse<SysExternalConnect>().SetError("connection not found");

        return new ApiResponse<SysExternalConnect>(data);
    }

    [HttpPost("by-ids")]
    public async Task<ApiResponse<SysUserDto[]>> ByIds([FromBody] string[] ids)
    {
        var data = await this._userProfileService.QueryUserProfileByIdsAsync(ids);
        return new ApiResponse<SysUserDto[]>().SetData(data);
    }
}