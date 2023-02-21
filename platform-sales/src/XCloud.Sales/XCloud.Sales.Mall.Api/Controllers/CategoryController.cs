using Microsoft.AspNetCore.Mvc;
using XCloud.Sales.ViewService;
using XCloud.Core.Cache;
using XCloud.Core.Dto;
using XCloud.Sales.Service.Catalog;

namespace XCloud.Sales.Mall.Api.Controllers;

[Route("api/mall/category")]
public class CategoryController : ShopBaseController
{
    private readonly ICategoryViewService _categoryViewService;

    public CategoryController(ICategoryViewService categoryViewService)
    {
        _categoryViewService = categoryViewService;
    }

    [HttpPost("view")]
    public async Task<ApiResponse<CategoryPageDto>> CategoryPageAsync()
    {
        var data = await this._categoryViewService.QueryCategoryPageDataAsync(new CachePolicy()
        {
            Cache = true
        });

        return new ApiResponse<CategoryPageDto>(data);
    }

    [HttpPost("by-parent")]
    public async Task<ApiResponse<CategoryDto[]>> ByParent([FromBody] IdDto<int?> dto)
    {
        var data = await this._categoryViewService.ByParentIdAsync(dto.Id, 
            new CachePolicy() { Cache = true });

        return new ApiResponse<CategoryDto[]>(data);
    }
}