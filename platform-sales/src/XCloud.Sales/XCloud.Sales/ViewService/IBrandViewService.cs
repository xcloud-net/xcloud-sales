using XCloud.Sales.Application;
using XCloud.Sales.Service.Catalog;

namespace XCloud.Sales.ViewService;

public interface IBrandViewService : ISalesAppService
{
    Task<BrandDto[]> QueryAllBrandsAsync(CachePolicy cachePolicy);
}

public class BrandViewService : SalesAppService, IBrandViewService
{
    private readonly IBrandService _brandService;

    public BrandViewService(IBrandService brandService)
    {
        _brandService = brandService;
    }

    public async Task<BrandDto[]> QueryAllBrandsAsync(CachePolicy cachePolicy)
    {
        var key = $"mall.brands.all.view";
        var option = new CacheOption<BrandDto[]>(key, TimeSpan.FromMinutes(5));

        var response = await this.CacheProvider.ExecuteWithPolicyAsync(
            this.QueryAllBrandsAsync, option, cachePolicy);

        if (response == null)
            response = Array.Empty<BrandDto>();

        return response;
    }

    private async Task<BrandDto[]> QueryAllBrandsAsync()
    {
        var dto = new QueryBrandDto
        {
            Published = true,
            ShowOnHomePage = true,
            StoreId = null,
            Page = 1,
            PageSize = 1000,
            SkipCalculateTotalCount = true
        };

        var response = await _brandService.QueryPagingAsync(dto);

        if (response.IsNotEmpty)
        {
            await this._brandService.AttachDataAsync(response.Items.ToArray(), new AttachBrandDataInput()
            {
                Picture = true
            });
        }

        return response.Items.ToArray();
    }
}