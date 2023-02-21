using Microsoft.AspNetCore.Mvc;
using XCloud.Core.Dto;
using XCloud.Sales.Data.Domain.Catalog;
using XCloud.Sales.Service.Authentication;
using XCloud.Sales.Service.Catalog;

namespace XCloud.Sales.Mall.Api.Controllers.Admin;

[StoreAuditLog]
[Route("api/mall-admin/goods-attr")]
public class GoodsAttributeController : ShopBaseController
{
    private readonly IGoodsService _goodsService;
    private readonly IGoodsAttributeService _goodsAttributeService;

    public GoodsAttributeController(IGoodsService goodsService,
        IGoodsAttributeService goodsAttributeService)
    {
        this._goodsService = goodsService;
        this._goodsAttributeService = goodsAttributeService;
    }

    [HttpPost("delete")]
    public async Task<ApiResponse<object>> DeleteAttributesAsync([FromBody] IdDto dto)
    {
        var storeAdministrator = await this.StoreAuthService.GetRequiredStoreAdministratorAsync();

        await this.SalesPermissionService.CheckRequiredPermissionAsync(storeAdministrator,
            SalesPermissions.ManageCatalog);

        var attr = await this._goodsAttributeService.QueryByIdAsync(dto.Id);
        if (attr == null)
            throw new EntityNotFoundException(nameof(DeleteAttributesAsync));

        await this._goodsAttributeService.DeleteByIdAsync(dto.Id);

        return new ApiResponse<object>();
    }

    [HttpPost("save")]
    public async Task<ApiResponse<object>> SaveAttributesAsync([FromBody] GoodsAttribute dto)
    {
        var storeAdministrator = await this.StoreAuthService.GetRequiredStoreAdministratorAsync();

        await this.SalesPermissionService.CheckRequiredPermissionAsync(storeAdministrator,
            SalesPermissions.ManageCatalog);

        if (string.IsNullOrWhiteSpace(dto.Id))
        {
            await this._goodsAttributeService.InsertAsync(dto);
        }
        else
        {
            await this._goodsAttributeService.UpdateAsync(dto);
        }

        return new ApiResponse<object>();
    }

    [HttpPost("set-goods-attributes")]
    public async Task<ApiResponse<object>> SetGoodsAttributesAsync([FromBody] SaveAttributesInput dto)
    {
        var storeAdministrator = await this.StoreAuthService.GetRequiredStoreAdministratorAsync();

        await this.SalesPermissionService.CheckRequiredPermissionAsync(storeAdministrator,
            SalesPermissions.ManageCatalog);

        await this._goodsAttributeService.SetGoodsAttributesAsync(dto.GoodsId, dto.GoodsAttributes);

        return new ApiResponse<object>();
    }

    [HttpPost("query-goods-attributes")]
    public async Task<ApiResponse<GoodsAttribute[]>> QueryGoodsAttributesAsync([FromBody] IdDto<int> dto)
    {
        var storeAdministrator = await this.StoreAuthService.GetRequiredStoreAdministratorAsync();

        await this.SalesPermissionService.CheckRequiredPermissionAsync(storeAdministrator,
            SalesPermissions.ManageCatalog);

        var data = await this._goodsAttributeService.QueryGoodsAttributesAsync(dto.Id);

        return new ApiResponse<GoodsAttribute[]>(data);
    }
}