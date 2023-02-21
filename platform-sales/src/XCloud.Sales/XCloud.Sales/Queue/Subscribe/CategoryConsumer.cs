using DotNetCore.CAP;
using XCloud.Sales.Application;
using XCloud.Sales.Service.Catalog;

namespace XCloud.Sales.Queue.Subscribe;

[UnitOfWork]
public class CategoryConsumer : SalesAppService, ICapSubscribe
{
    private readonly ICategoryService _categoryService;
    public CategoryConsumer(
        ICategoryService categoryService)
    {
        this._categoryService = categoryService;
    }
}