using XCloud.Sales.Data.Domain.Catalog;

namespace XCloud.Sales.Service.Catalog;

public static class CatalogExtension
{
    public static async Task<int[]> QueryCategoryAndAllChildrenIdsWithCacheAsync(this ICategoryService categoryService,
        int catId)
    {
        var categories = new List<Category>();
        var handledIds = new List<int>();

        var cachePolicyOption = new CachePolicy() { Cache = true };

        async Task FindChildren(Category cat)
        {
            if (handledIds.Contains(cat.Id))
                return;
            handledIds.Add(cat.Id);

            categories.Add(cat);

            var children = await categoryService.QueryByParentIdAsync(cat.Id, cachePolicyOption);

            foreach (var m in children)
                await FindChildren(m);
        }

        var category = await categoryService.QueryByIdAsync(catId, cachePolicyOption);
        if (category != null)
        {
            await FindChildren(category);
        }

        return categories.Select(x => x.Id).Distinct().ToArray();
    }
}