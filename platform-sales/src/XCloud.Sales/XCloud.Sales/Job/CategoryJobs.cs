using XCloud.Logging;
using XCloud.Sales.Application;
using XCloud.Sales.Service.Catalog;

namespace XCloud.Sales.Job;

[UnitOfWork]
[LogExceptionSilence]
public class CategoryJobs : SalesAppService, ITransientDependency
{
    private readonly ICategoryService _categoryService;

    public CategoryJobs(ICategoryService categoryService)
    {
        this._categoryService = categoryService;
    }

    [LogExceptionSilence]
    public virtual async Task TryFixCategoryTreeAsync()
    {
        await this._categoryService.TryFixTreeAsync();
    }
}