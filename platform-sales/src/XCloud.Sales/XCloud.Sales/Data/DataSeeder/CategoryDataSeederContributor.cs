using Volo.Abp.Data;
using XCloud.Sales.Application;
using XCloud.Sales.Data.Domain.Catalog;
using XCloud.Sales.Service.Catalog;

namespace XCloud.Sales.Data.DataSeeder;

public class CategoryDataSeederContributor : SalesAppService, IDataSeedContributor, ITransientDependency
{
    private readonly ICategoryService _categoryService;
    private readonly ISalesRepository<Category> _repository;

    public CategoryDataSeederContributor(ICategoryService categoryService, ISalesRepository<Category> repository)
    {
        _categoryService = categoryService;
        _repository = repository;
    }

    public async Task SeedAsync(DataSeedContext context)
    {
        using var dlock = await this.RedLockClient.RedLockFactory.CreateLockAsync(
            resource: $"try-create-preset-categories",
            expiryTime: TimeSpan.FromSeconds(30));

        if (dlock.IsAcquired)
        {
            var db = await this._repository.GetDbContextAsync();
            var query = db.Set<Category>().IgnoreQueryFilters().AsNoTracking();

            if (await query.AnyAsync())
                return;

            var presetCategories = new[] { "洗护", "护肤", "彩妆", "护手", "口红", "套盒", "小样" };

            foreach (var m in presetCategories)
            {
                await this._categoryService.InsertCategoryAsync(new Category()
                {
                    ParentCategoryId = default,
                    Name = m,
                    IsDeleted = false,
                    Published = true,
                });
            }

            await Task.CompletedTask;
        }
        else
        {
            this.Logger.LogWarning("failed to get distributed lock");
        }
    }
}