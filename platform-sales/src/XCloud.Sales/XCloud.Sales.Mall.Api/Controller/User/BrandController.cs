using Microsoft.AspNetCore.Mvc;
using XCloud.Core.Cache;
using XCloud.Core.Dto;
using XCloud.Sales.Service.Catalog;
using XCloud.Sales.ViewService;

namespace XCloud.Sales.Mall.Api.Controller.User;

[Route("api/mall/brand")]
public class BrandController : ShopBaseController
{
    private readonly IBrandViewService _brandViewService;

    public BrandController(IBrandViewService brandViewService)
    {
        _brandViewService = brandViewService;
    }

    [HttpPost("all")]
    public async Task<ApiResponse<BrandDto[]>> AllAsync()
    {
        var brands = await this._brandViewService.QueryAllBrandsAsync(new CachePolicy() { Cache = true });

        return new ApiResponse<BrandDto[]>(brands);
    }

    [Obsolete]
    [HttpPost("list")]
    public async Task<ApiResponse<BrandDto[]>> ListAsync([FromBody] QueryBrandDto dto)
    {
        var brands = await this._brandViewService.QueryAllBrandsAsync(new CachePolicy() { Cache = true });

        return new ApiResponse<BrandDto[]>(brands);
    }
}