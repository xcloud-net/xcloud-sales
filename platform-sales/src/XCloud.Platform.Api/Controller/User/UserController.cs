using FluentAssertions;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.IO;
using System.Threading.Tasks;
using XCloud.AspNetMvc.ModelBinder.JsonModel;
using XCloud.Core.Cache;
using XCloud.Core.Dto;
using XCloud.Core.Extension;
using XCloud.Core.Helper;
using XCloud.Platform.Auth.Application.User;
using XCloud.Platform.Framework.Controller;
using XCloud.Platform.Member.Application.Extension;
using XCloud.Platform.Member.Application.Service.User;

namespace XCloud.Platform.Api.Controller.User;

[Route("/api/platform/user")]
public class UserController : PlatformBaseController, IUserController
{
    private readonly IUserProfileService _userProfileService;
    private readonly IWebHostEnvironment _webHostEnvironment;
    private readonly IMemoryCache _memoryCache;

    public UserController(IUserProfileService userService,
        IWebHostEnvironment webHostEnvironment,
        IMemoryCache memoryCache)
    {
        this._userProfileService = userService;
        this._webHostEnvironment = webHostEnvironment;
        this._memoryCache = memoryCache;
    }

    private readonly Random _rand = new Random((int)DateTime.UtcNow.Ticks);

    [HttpGet("random-avatar")]
    public async Task<IActionResult> RandomAvatars()
    {
        var dir = Path.Combine(this._webHostEnvironment.ContentRootPath, "wwwroot", "images", "avatars");

        var files = this._memoryCache.GetOrCreate($"{nameof(RandomAvatars)}.avatars", x =>
        {
            x.AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(30);
            return System.IO.Directory.GetFiles(dir, "*.svg");
        });

        if (ValidateHelper.IsEmptyCollection(files))
            return this.NotFound();

        var theOne = this._rand.Choice(files);

        var theOnePath = Path.Combine(dir, theOne);

        var bs = await System.IO.File.ReadAllBytesAsync(theOnePath);

        return this.File(bs, contentType: "image/svg+xml");
    }

    [HttpPost("profile")]
    public async Task<ApiResponse<SysUserDto>> UserProfile()
    {
        var loginuser = await this.GetRequiredAuthedUserAsync();

        var profileDto = await this._userProfileService.QueryProfileByUserIdAsync(new IdDto(loginuser.UserId),
            new CachePolicy() { Cache = true });

        if (profileDto == null)
            return new ApiResponse<SysUserDto>().SetError("user not exist");

        profileDto.HideSensitiveInformation();

        return new ApiResponse<SysUserDto>().SetData(profileDto);
    }

    /// <summary>
    /// 更新个人信息
    /// </summary>
    [HttpPost("update-profile")]
    public async Task<ApiResponse<object>> UpdateProfile([JsonData] SysUserDto model)
    {
        var loginuser = await this.GetRequiredAuthedUserAsync();

        model.Id = loginuser.UserId;

        await this._userProfileService.UpdateProfileAsync(model);

        return new ApiResponse<object>();
    }

    /// <summary>
    /// 更新头像
    /// </summary>
    [HttpPost("update-avatar")]
    public async Task<ApiResponse<object>> UpdateAvatar([JsonData] UpdateUserAvatarDto model)
    {
        model.Should().NotBeNull();

        var loginuser = await this.GetRequiredAuthedUserAsync();

        await this._userProfileService.UpdateAvatarAsync(loginuser.UserId, model.AvatarUrl);

        return new ApiResponse<object>();
    }
}