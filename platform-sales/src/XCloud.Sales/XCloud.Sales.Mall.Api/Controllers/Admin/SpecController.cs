using Microsoft.AspNetCore.Mvc;
using XCloud.Core.Dto;
using XCloud.Sales.Data.Domain.Catalog;
using XCloud.Sales.Services.Catalog;

namespace XCloud.Sales.Mall.Api.Controllers.Admin;

[StoreAuditLog]
[Route("api/mall-admin/spec")]
public class SpecController : ShopBaseController
{
    private readonly ISpecService _goodsSpecService;

    public SpecController(ISpecService goodsSpecService)
    {
        this._goodsSpecService = goodsSpecService;
    }

    [HttpPost("list")]
    public async Task<ApiResponse<SpecDto[]>> ListJson([FromBody] IdDto<int> dto)
    {
        var storeAdministrator = await this.StoreAuthService.GetRequiredStoreAdministratorAsync();

        await this.SalesPermissionService.CheckRequiredPermissionAsync(storeAdministrator,
            SalesPermissions.ManageCatalog);

        var goodsSpecs = await _goodsSpecService.QuerySpecByGoodsIdAsync(dto.Id);

        goodsSpecs = await this._goodsSpecService.AttachDataAsync(goodsSpecs, new GoodsSpecAttachDataInput() { Values = true });

        return new ApiResponse<SpecDto[]>(goodsSpecs);
    }

    [HttpPost("save")]
    public virtual async Task<ApiResponse<object>> Edit([FromBody] Spec model)
    {
        var storeAdministrator = await this.StoreAuthService.GetRequiredStoreAdministratorAsync();

        await this.SalesPermissionService.CheckRequiredPermissionAsync(storeAdministrator,
            SalesPermissions.ManageCatalog);

        if (model.Id <= 0)
        {
            await _goodsSpecService.InsertSpecAsync(model);
        }
        else
        {
            await _goodsSpecService.UpdateSpecAsync(model);
        }

        return new ApiResponse<object>();
    }

    //delete
    [HttpPost("delete")]
    public virtual async Task<ApiResponse<object>> Delete([FromBody] IdDto<int> dto)
    {
        var storeAdministrator = await this.StoreAuthService.GetRequiredStoreAdministratorAsync();

        await this.SalesPermissionService.CheckRequiredPermissionAsync(storeAdministrator,
            SalesPermissions.ManageCatalog);

        await this._goodsSpecService.UpdateSpecStatusAsync(new UpdateGoodsSpecStatusInput() { Id = dto.Id, IsDeleted = true });

        return new ApiResponse<object>();
    }

    [HttpPost("save-value")]
    public virtual async Task<ApiResponse<object>> ValueEdit([FromBody] SpecValue model)
    {
        var storeAdministrator = await this.StoreAuthService.GetRequiredStoreAdministratorAsync();

        await this.SalesPermissionService.CheckRequiredPermissionAsync(storeAdministrator,
            SalesPermissions.ManageCatalog);

        if (model.Id <= 0)
        {
            await _goodsSpecService.InsertSpecValueAsync(model);
        }
        else
        {
            await _goodsSpecService.UpdateSpecValueAsync(model);
        }

        return new ApiResponse<object>();
    }

    [HttpPost("delete-value")]
    public virtual async Task<ApiResponse<object>> ValueDelete([FromBody] IdDto<int> dto)
    {
        var storeAdministrator = await this.StoreAuthService.GetRequiredStoreAdministratorAsync();

        await this.SalesPermissionService.CheckRequiredPermissionAsync(storeAdministrator,
            SalesPermissions.ManageCatalog);

        await this._goodsSpecService.UpdateSpecValueStatusAsync(new UpdateGoodsSpecValueStatusInput() { Id = dto.Id, IsDeleted = true });

        return new ApiResponse<object>();
    }

}