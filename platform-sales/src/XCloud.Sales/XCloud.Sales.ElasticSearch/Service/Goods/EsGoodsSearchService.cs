using System.Threading.Tasks;
using Volo.Abp.DependencyInjection;
using XCloud.Core.Dto;
using XCloud.Sales.Application;
using XCloud.Sales.Data.Domain.Catalog;
using XCloud.Sales.Service.Catalog;
using XCloud.Sales.Service.Search;

namespace XCloud.Sales.ElasticSearch.Service.Goods;

[Dependency(ReplaceServices = true)]
public class EsGoodsSearchService : SalesAppService, IGoodsSearchService
{
    public Task UpdateGoodsKeywordsAsync(int goodsId)
    {
        throw new System.NotImplementedException();
    }

    public Task<Tag[]> QueryRelatedTagsAsync(SearchProductsInput dto)
    {
        throw new System.NotImplementedException();
    }

    public Task<Brand[]> QueryRelatedBrandsAsync(SearchProductsInput dto)
    {
        throw new System.NotImplementedException();
    }

    public Task<Category[]> QueryRelatedCategoriesAsync(SearchProductsInput dto)
    {
        throw new System.NotImplementedException();
    }

    public Task<PagedResponse<GoodsDto>> SearchGoodsV2Async(SearchProductsInput dto)
    {
        throw new System.NotImplementedException();
    }

    public Task<PagedResponse<GoodsSpecCombinationDto>> QueryGoodsCombinationPaginationAsync(
        QueryGoodsCombinationInput dto)
    {
        throw new System.NotImplementedException();
    }
}