using Microsoft.AspNetCore.Mvc;
using XCloud.Core.Dto;
using XCloud.Sales.Service.Authentication;
using XCloud.Sales.Service.Catalog;

namespace XCloud.Sales.Mall.Api.Controller.Admin;

[StoreAuditLog]
[Route("api/mall-admin/brand")]
public class BrandController : ShopBaseController
{
    private readonly IBrandService _brandService;

    public BrandController(IBrandService brandService)
    {
        this._brandService = brandService;
    }

    [HttpPost("set-picture")]
    public async Task<ApiResponse<object>> SetPictureAsync([FromBody] SetBrandPictureIdInput dto)
    {
        var storeAdministrator = await this.StoreAuthService.GetRequiredStoreAdministratorAsync();

        await this.SalesPermissionService.CheckRequiredPermissionAsync(storeAdministrator,
            SalesPermissions.ManageBrands);

        await this._brandService.SetPictureIdAsync(dto.Id, dto.PictureId);

        return new ApiResponse<object>();
    }

    [HttpPost("list")]
    public async Task<ApiResponse<BrandDto[]>> ListBrand([FromBody] QueryBrandDto dto)
    {
        var storeAdministrator = await this.StoreAuthService.GetRequiredStoreAdministratorAsync();

        await this.SalesPermissionService.CheckRequiredPermissionAsync(storeAdministrator,
            SalesPermissions.ManageBrands);

        dto.Page = 1;
        dto.PageSize = 1000;
        dto.SkipCalculateTotalCount = true;

        var response = await _brandService.QueryPagingAsync(dto);

        if (response.IsNotEmpty)
        {
            await this._brandService.AttachDataAsync(response.Items.ToArray(), new AttachBrandDataInput() { Picture = true });
        }

        return new ApiResponse<BrandDto[]>(response.Items.ToArray());
    }

    [HttpPost("paging")]
    public async Task<PagedResponse<BrandDto>> Paging([FromBody] QueryBrandDto model)
    {
        var storeAdministrator = await this.StoreAuthService.GetRequiredStoreAdministratorAsync();

        await this.SalesPermissionService.CheckRequiredPermissionAsync(storeAdministrator,
            SalesPermissions.ManageBrands);

        model.PageSize = 20;
        model.StoreId = null;
        var brands = await _brandService.QueryPagingAsync(model);

        if (brands.IsNotEmpty)
        {
            await this._brandService.AttachDataAsync(brands.Items.ToArray(),
                new AttachBrandDataInput() { Picture = true });
        }

        return brands;
    }
    
    [HttpPost("save")]
    public virtual async Task<ApiResponse<object>> SaveBrandAsync([FromBody] BrandDto model)
    {
        var storeAdministrator = await this.StoreAuthService.GetRequiredStoreAdministratorAsync();

        await this.SalesPermissionService.CheckRequiredPermissionAsync(storeAdministrator,
            SalesPermissions.ManageBrands);

        if (model.Id > 0)
        {
            var brand = await this._brandService.QueryByIdAsync(model.Id);
            
            if (brand == null)
                throw new EntityNotFoundException(nameof(brand));

            await this._brandService.UpdateAsync(model);
        }
        else
        {
            await this._brandService.InsertAsync(model);
        }

        return new ApiResponse<object>();
    }

    [HttpPost("update-status")]
    public virtual async Task<ApiResponse<object>> UpdateStatus([FromBody] UpdateBrandStatusInput dto)
    {
        var storeAdministrator = await this.StoreAuthService.GetRequiredStoreAdministratorAsync();

        await this.SalesPermissionService.CheckRequiredPermissionAsync(storeAdministrator,
            SalesPermissions.ManageBrands);

        var brand = await _brandService.QueryByIdAsync(dto.BrandId);

        if (brand == null)
            throw new EntityNotFoundException(nameof(brand));

        await this._brandService.UpdateStatusAsync(dto);

        return new ApiResponse<object>();
    }
}