using Microsoft.AspNetCore.Mvc;
using XCloud.Core.Dto;
using XCloud.Sales.Data.Domain.Catalog;
using XCloud.Sales.Service.Authentication;
using XCloud.Sales.Service.Catalog;

namespace XCloud.Sales.Mall.Api.Controller.Admin;

[StoreAuditLog]
[Route("api/mall-admin/category")]
public class CategoryController : ShopBaseController
{
    private readonly ICategoryService _categoryService;

    public CategoryController(ICategoryService categoryService)
    {
        this._categoryService = categoryService;
    }

    [HttpPost("set-picture")]
    public async Task<ApiResponse<object>> SetPictureAsync([FromBody] SetCategoryPictureIdInput dto)
    {
        var storeAdministrator = await this.StoreAuthService.GetRequiredStoreAdministratorAsync();

        await this.SalesPermissionService.CheckRequiredPermissionAsync(storeAdministrator,
            SalesPermissions.ManageCatalog);

        await this._categoryService.SetPictureIdAsync(dto.Id, dto.PictureId);

        return new ApiResponse<object>();
    }

    [HttpPost("tree")]
    public async Task<ApiResponse<AntDesignTreeNode[]>> Tree()
    {
        var storeAdministrator = await this.StoreAuthService.GetRequiredStoreAdministratorAsync();

        await this.SalesPermissionService.CheckRequiredPermissionAsync(storeAdministrator,
            SalesPermissions.ManageCatalog);

        var data = await this._categoryService.QueryAllAsync(null);

        var dataDto = data.Select(x => this.ObjectMapper.Map<Category, CategoryDto>(x)).ToArray();

        await this._categoryService.AttachDataAsync(dataDto, new CategoryAttachDataInput()
        {
            Picture = true
        });

        var tree = dataDto.BuildAntTree(
            x => x.Id.ToString(),
            x => x.ParentCategoryId > 0 ? x.ParentCategoryId.ToString() : string.Empty,
            x => x.Name).ToArray();

        return new ApiResponse<AntDesignTreeNode[]>(tree);
    }

    [HttpPost("save")]
    public virtual async Task<ApiResponse<object>> Edit([FromBody] Category model)
    {
        var storeAdministrator = await this.StoreAuthService.GetRequiredStoreAdministratorAsync();

        await this.SalesPermissionService.CheckRequiredPermissionAsync(storeAdministrator,
            SalesPermissions.ManageCatalog);

        if (model.Id > 0)
        {
            await this._categoryService.UpdateCategoryAsync(model);
        }
        else
        {
            await this._categoryService.InsertCategoryAsync(model);
        }

        return new ApiResponse<object>();
    }

    [HttpPost("update-status")]
    public virtual async Task<ApiResponse<object>> Delete([FromBody] UpdateCategoryStatusInput dto)
    {
        var storeAdministrator = await this.StoreAuthService.GetRequiredStoreAdministratorAsync();

        await this.SalesPermissionService.CheckRequiredPermissionAsync(storeAdministrator,
            SalesPermissions.ManageCatalog);

        await _categoryService.UpdateStatusAsync(dto);

        return new ApiResponse<object>();
    }
}