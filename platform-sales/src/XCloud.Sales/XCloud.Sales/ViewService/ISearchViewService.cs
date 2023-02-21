using XCloud.Sales.Application;
using XCloud.Sales.Data;
using XCloud.Sales.Data.Domain.Catalog;
using XCloud.Sales.Service.Catalog;
using XCloud.Sales.Service.Report;

namespace XCloud.Sales.ViewService;

public interface ISearchViewService : ISalesAppService
{
    Task<SearchView> QuerySearchViewAsync(CachePolicy cachePolicy);
}

public class SearchViewService : SalesAppService, ISearchViewService
{
    private readonly IGoodsReportService _goodsReportService;
    private readonly IGoodsCollectionService _goodsCollectionService;
    private readonly ITagService _tagService;
    private readonly ISalesRepository<Brand> _repository;

    public SearchViewService(IGoodsReportService goodsReportService,
        IGoodsCollectionService goodsCollectionService,
        ITagService tagService, ISalesRepository<Brand> repository)
    {
        this._repository = repository;
        this._tagService = tagService;
        this._goodsCollectionService = goodsCollectionService;
        this._goodsReportService = goodsReportService;
    }

    private async Task<BrandDto[]> SuggestBrandsAsync()
    {
        var db = await this._repository.GetDbContextAsync();

        var query = from goods in db.Set<Goods>().AsNoTracking()
            join brand in db.Set<Brand>().AsNoTracking()
                on goods.BrandId equals brand.Id
            select new { goods, brand };

        var groupedQuery = query.GroupBy(x => new
        {
            x.brand.Id
        }).Select(x => new
        {
            x.Key.Id,
            count = x.Count()
        });

        var data = await groupedQuery.OrderByDescending(x => x.count).Take(10).ToArrayAsync();

        if (!data.Any())
            return Array.Empty<BrandDto>();

        var ids = data.Select(x => x.Id).ToArray();
        var brands = await db.Set<Brand>().AsNoTracking().WhereIdIn(ids).ToArrayAsync();

        var q = from d in data join b in brands on d.Id equals b.Id select new { b, d.count };

        var response = q.OrderByDescending(x => x.count).Select(x => this.ObjectMapper.Map<Brand, BrandDto>(x.b))
            .ToArray();

        return response;
    }

    private async Task<SearchView> QuerySearchViewAsync()
    {
        var keywords =
            await this._goodsReportService.QuerySearchKeywordsReportAsync(new QuerySearchKeywordsReportInput()
                { MaxCount = 5 });

        var dto = new SearchView
        {
            Keywords = keywords.Select(x => x.Keywords).ToArray(),
            Collections = await this._goodsCollectionService.QueryAllAsync(),
            Brands = await this.SuggestBrandsAsync(),
            Tags = await this._tagService.QueryAllAsync()
        };

        return dto;
    }

    public async Task<SearchView> QuerySearchViewAsync(CachePolicy cachePolicy)
    {
        if (cachePolicy == null)
            throw new ArgumentNullException(nameof(cachePolicy));

        var key = $"mall.search.view.data";
        var option = new CacheOption<SearchView>(key, TimeSpan.FromMinutes(30));

        var data = await this.CacheProvider.ExecuteWithPolicyAsync(
            this.QuerySearchViewAsync,
            option,
            cachePolicy);

        data = data ?? new SearchView();

        return data;
    }
}