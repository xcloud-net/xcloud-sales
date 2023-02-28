using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Volo.Abp.Domain.Entities;
using XCloud.Core.Cache;
using XCloud.Core.Dto;
using XCloud.Platform.Auth.Application.User;
using XCloud.Platform.Common.Application.Service.Notification;
using XCloud.Platform.Framework.Controller;

namespace XCloud.Platform.Api.Controller.User;

[Route("/api/platform/user/notification")]
public class UserNotificationController : PlatformBaseController, IUserController
{
    private readonly INotificationService _notificationService;

    public UserNotificationController(INotificationService notificationService)
    {
        this._notificationService = notificationService;
    }

    [HttpPost("update-status")]
    public async Task<ApiResponse<object>> UpdateStatusAsync([FromBody] UpdateNotificationStatusInput dto)
    {
        var loginUser = await this.GetRequiredAuthedUserAsync();

        var entity = await this._notificationService.QueryByIdAsync(dto.Id);

        if (entity == null || entity.UserId != loginUser.Id)
            throw new EntityNotFoundException(nameof(DeleteNotification));

        await this._notificationService.UpdateNotificationStatusAsync(dto);

        return new ApiResponse<object>();
    }

    [HttpPost("paging")]
    public async Task<PagedResponse<SysNotificationDto>> Paging([FromBody] QueryNotificationInput dto)
    {
        var loginUser = await this.GetRequiredAuthedUserAsync();
        dto.UserId = loginUser.UserId;

        var res = await this._notificationService.QueryPaginationAsync(dto);

        return res;
    }

    [HttpPost("delete")]
    public async Task<ApiResponse<object>> DeleteNotification([FromBody] IdDto dto)
    {
        var loginUser = await this.GetRequiredAuthedUserAsync();

        var entity = await this._notificationService.QueryByIdAsync(dto.Id);

        if (entity == null || entity.UserId != loginUser.Id)
            throw new EntityNotFoundException(nameof(DeleteNotification));

        await this._notificationService.DeleteAsync(dto.Id);

        return new ApiResponse<object>();
    }

    [HttpPost("unread-count")]
    public async Task<ApiResponse<int>> UnReadCountAsync()
    {
        var loginUser = await this.GetRequiredAuthedUserAsync();

        var count = await this._notificationService.UnreadCountAsync(loginUser.UserId,
            new CachePolicy() { Cache = true });

        return new ApiResponse<int>(count);
    }
}