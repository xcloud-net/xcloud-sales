using Microsoft.AspNetCore.Mvc;
using XCloud.Core.Dto;
using XCloud.Sales.Service.Authentication;
using XCloud.Sales.Service.Common;

namespace XCloud.Sales.Mall.Api.Controller.Admin;

[StoreAuditLog]
[Route("api/mall-admin/pages")]
public class PagesController : ShopBaseController
{
    private readonly IPagesService _pagesService;

    public PagesController(IPagesService pagesService)
    {
        this._pagesService = pagesService;
    }

    [HttpPost("paging")]
    public async Task<PagedResponse<PagesDto>> PagingAsync([FromBody] QueryPagesInput dto)
    {
        var storeAdministrator = await this.StoreAuthService.GetRequiredStoreAdministratorAsync();

        await this.SalesPermissionService.CheckRequiredPermissionAsync(storeAdministrator,
            SalesPermissions.ManagePages);

        dto.PageSize = 20;
        dto.IsDeleted = null;
        dto.SortForAdmin = true;

        var res = await this._pagesService.QueryPagingAsync(dto);

        await this._pagesService.AttachDataAsync(res.Items.ToArray(), new AttachPageDataInput() { CoverImage = true });

        return res;
    }

    [HttpPost("set-content")]
    public async Task<ApiResponse<object>> SetContentAsync([FromBody] SetPageContentInput dto)
    {
        var storeAdministrator = await this.StoreAuthService.GetRequiredStoreAdministratorAsync();

        await this.SalesPermissionService.CheckRequiredPermissionAsync(storeAdministrator,
            SalesPermissions.ManagePages);

        await this._pagesService.SetPageContentAsync(dto);

        return new ApiResponse<object>();
    }

    [HttpPost("save")]
    public async Task<ApiResponse<object>> SaveAsync([FromBody] PagesDto dto)
    {
        var storeAdministrator = await this.StoreAuthService.GetRequiredStoreAdministratorAsync();

        await this.SalesPermissionService.CheckRequiredPermissionAsync(storeAdministrator,
            SalesPermissions.ManagePages);

        if (string.IsNullOrWhiteSpace(dto.Id))
        {
            await this._pagesService.InsertAsync(dto);
        }
        else
        {
            await this._pagesService.UpdateAsync(dto);
        }

        return new ApiResponse<object>();
    }

    [HttpPost("update-status")]
    public async Task<ApiResponse<object>> UpdateStatusAsync([FromBody] UpdatePagesStatusInput dto)
    {
        var storeAdministrator = await this.StoreAuthService.GetRequiredStoreAdministratorAsync();

        await this.SalesPermissionService.CheckRequiredPermissionAsync(storeAdministrator,
            SalesPermissions.ManagePages);

        await this._pagesService.UpdatePagesStatusAsync(dto);
        return new ApiResponse<object>();
    }
}