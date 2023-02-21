using XCloud.Sales.Application;
using XCloud.Sales.Data.Domain.Catalog;
using XCloud.Sales.Service.Catalog;

namespace XCloud.Sales.ViewService;

public interface ICategoryViewService : ISalesAppService
{
    Task<CategoryDto[]> ByParentIdAsync(int? parentId, CachePolicy cachePolicy);
    
    Task<CategoryPageDto> QueryCategoryPageDataAsync(CachePolicy cachePolicyOption);
}

public class CategoryViewService : SalesAppService, ICategoryViewService
{
    private readonly ICategoryService _categoryService;

    public CategoryViewService(ICategoryService categoryService)
    {
        this._categoryService = categoryService;
    }

    private async Task<CategoryDto[]> ByParentIdAsync(int? parentId)
    {
        var data = await this._categoryService.QueryByParentIdAsync(parentId);
        var response = data.Select(x => this.ObjectMapper.Map<Category, CategoryDto>(x)).ToArray();
        if (response.Any())
        {
            await this._categoryService.AttachDataAsync(response, new CategoryAttachDataInput()
            {
                Picture = true
            });
        }

        response = response.OrderByDescending(x => x.DisplayOrder).ThenBy(x => x.CreationTime).ToArray();
        return response;
    }

    public async Task<CategoryDto[]> ByParentIdAsync(int? parentId, CachePolicy cachePolicy)
    {
        var key = $"view.by.parent.id=${parentId ?? 0}";
        var option = new CacheOption<CategoryDto[]>(key, TimeSpan.FromMinutes(5));

        var children =
            await this.CacheProvider.ExecuteWithPolicyAsync(() => this.ByParentIdAsync(parentId), option, cachePolicy);

        children ??= Array.Empty<CategoryDto>();

        return children;
    }

    private async Task<CategoryPageDto> QueryRootCategoryAsync()
    {
        var dto = new CategoryPageDto
        {
            Root = await this.ByParentIdAsync(null)
        };

        return dto;
    }

    public async Task<CategoryPageDto> QueryCategoryPageDataAsync(CachePolicy cachePolicyOption)
    {
        var key = $"category.root.data";
        var option = new CacheOption<CategoryPageDto>(key, TimeSpan.FromMinutes(10));

        var response =
            await this.CacheProvider.ExecuteWithPolicyAsync(this.QueryRootCategoryAsync, option, cachePolicyOption);

        response ??= new CategoryPageDto();

        return response;
    }
}