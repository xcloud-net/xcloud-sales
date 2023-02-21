using Microsoft.AspNetCore.Mvc;
using XCloud.Core.Dto;
using XCloud.Sales.Service.Authentication;
using XCloud.Sales.Service.Catalog;
using XCloud.Sales.Service.Logging;

namespace XCloud.Sales.Mall.Api.Controllers.Admin;

[StoreAuditLog]
[Route("api/mall-admin/common")]
public class CommonController : ShopBaseController
{
    private readonly ITagService _tagService;
    private readonly IActivityLogService _activityLogService;

    public CommonController(ITagService tagService, IActivityLogService activityLogService)
    {
        this._tagService = tagService;
        this._activityLogService = activityLogService;
    }

    [HttpPost("log-paging")]
    public async Task<PagedResponse<ActivityLogDto>> LogPagingAsync([FromBody] ActivityLogSearchInput dto)
    {
        var storeAdministrator = await this.StoreAuthService.GetRequiredStoreAdministratorAsync();

        await this.SalesPermissionService.CheckRequiredPermissionAsync(storeAdministrator,
            SalesPermissions.ManageActivityLog);

        var response = await this._activityLogService.QueryPagingAsync(dto);

        return response;
    }

    [HttpPost("delete-log")]
    public async Task<ApiResponse<object>> DeleteLogAsync([FromBody] IdDto<int> dto)
    {
        var storeAdministrator = await this.StoreAuthService.GetRequiredStoreAdministratorAsync();

        await this.SalesPermissionService.CheckRequiredPermissionAsync(storeAdministrator,
            SalesPermissions.ManageActivityLog);

        await this._activityLogService.DeleteAsync(dto.Id);

        return new ApiResponse<object>();
    }

    [HttpPost("list-tags")]
    public virtual async Task<ApiResponse<TagDto[]>> ListTagsAsync()
    {
        var storeAdministrator = await this.StoreAuthService.GetRequiredStoreAdministratorAsync();

        await this.SalesPermissionService.CheckRequiredPermissionAsync(storeAdministrator,
            SalesPermissions.ManageCatalog);

        var tags = await this._tagService.QueryAllAsync();

        return new ApiResponse<TagDto[]>(tags);
    }

    [HttpPost("save-tag")]
    public virtual async Task<ApiResponse<object>> AddTagAsync([FromBody] TagDto dto)
    {
        var storeAdministrator = await this.StoreAuthService.GetRequiredStoreAdministratorAsync();

        await this.SalesPermissionService.CheckRequiredPermissionAsync(storeAdministrator,
            SalesPermissions.ManageCatalog);

        if (string.IsNullOrWhiteSpace(dto.Id))
        {
            await this._tagService.InsertAsync(dto);
        }
        else
        {
            await this._tagService.UpdateAsync(dto);
        }

        return new ApiResponse<object>();
    }

    [HttpPost("update-tag-status")]
    public virtual async Task<ApiResponse<object>> UpdateTagStatusAsync([FromBody] UpdateTagStatusInput dto)
    {
        var storeAdministrator = await this.StoreAuthService.GetRequiredStoreAdministratorAsync();

        await this.SalesPermissionService.CheckRequiredPermissionAsync(storeAdministrator,
            SalesPermissions.ManageCatalog);

        await this._tagService.UpdateStatusAsync(dto);

        return new ApiResponse<object>();
    }
}