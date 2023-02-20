using Microsoft.AspNetCore.Mvc;
using XCloud.Core.Dto;
using XCloud.Sales.Data.Domain.Catalog;
using XCloud.Sales.Data.Domain.Logging;
using XCloud.Sales.Services.Catalog;
using XCloud.Sales.Services.Logging;
using XCloud.Sales.Services.Media;

namespace XCloud.Sales.Mall.Api.Controllers;

[Route("api/mall/common")]
public class CommonController : ShopBaseController
{
    private readonly IPictureService _pictureService;
    private readonly IActivityLogService _activityLogService;
    private readonly ITagService _tagService;
    
    public CommonController(IPictureService pictureService,
        ITagService tagService,
        IActivityLogService activityLogService)
    {
        this._tagService = tagService;
        this._pictureService = pictureService;
        this._activityLogService = activityLogService;
    }

    [HttpPost("list-tags")]
    public virtual async Task<ApiResponse<TagDto[]>> ListTagsAsync()
    {
        var tags = await this._tagService.QueryAllAsync();

        return new ApiResponse<TagDto[]>(tags);
    }

    [HttpPost("query-tag-by-id")]
    public async Task<ApiResponse<Tag>> QueryTagByIdAsync([FromBody] IdDto dto)
    {
        var tag = await this._tagService.QueryByIdAsync(dto.Id);

        return new ApiResponse<Tag>(tag);
    }

    [HttpPost("save-activity-log")]
    public async Task<ApiResponse<object>> SaveActivityLogAsync([FromBody] ActivityLog log)
    {
        var storeUser = await this.StoreAuthService.GetStoreUserOrNullAsync();
        if (storeUser != null && log != null)
        {
            log.UserId = storeUser.Id;
            log = this._activityLogService.AttachHttpContextInfo(log);
            await this.EventBusService.NotifyInsertActivityLog(log);
        }

        return new ApiResponse<object>();
    }

    [HttpPost("save-pictures")]
    public async Task<ApiResponse<PictureDto[]>> SavePicture([FromBody] PictureDto[] picture)
    {
        var storeUser = await this.StoreAuthService.GetRequiredStoreUserAsync();

        var res = await this._pictureService.SavePictureAsync(picture);

        return new ApiResponse<PictureDto[]>(res);
    }
}