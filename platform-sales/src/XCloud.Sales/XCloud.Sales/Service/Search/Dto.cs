using Volo.Abp.Application.Dtos;
using XCloud.Core.Dto;
using XCloud.Sales.Data.Domain.Catalog;
using XCloud.Sales.Service.Catalog;

namespace XCloud.Sales.Service.Search;

public class SearchProductsInput : PagedRequest, IEntityDto
{
    public SearchProductsInput()
    {
        this.IsDeleted = false;
    }

    public int? StockQuantityLessThanOrEqualTo { get; set; }
    public int? StockQuantityGreaterThanOrEqualTo { get; set; }

    public bool? IsNew { get; set; }
    public bool? IsHot { get; set; }

    public bool? WithoutBrand { get; set; }
    public bool? WithoutCategory { get; set; }
    public string TagId { get; set; }
    public string StoreId { get; set; }
    public int? CategoryId { get; set; }
    public int? BrandId { get; set; }
    public decimal? PriceMin { get; set; }
    public decimal? PriceMax { get; set; }
    public string Keywords { get; set; }
    public GoodsSortingEnum OrderBy { get; set; }
    public bool? IsPublished { get; set; }
    public string Sku { get; set; }

    public AttachGoodsDataInput AttachDataOptions { get; set; }

    public bool? IsDeleted { get; set; } = false;
}

public class QueryGoodsCombinationInput : PagedRequest, IEntityDto
{
    public QueryGoodsCombinationInput()
    {
        this.IsDeleted = false;
    }

    public int? StockQuantityLessThanOrEqualTo { get; set; }
    public int? StockQuantityGreaterThanOrEqualTo { get; set; }

    public decimal? PriceMin { get; set; }
    public decimal? PriceMax { get; set; }
    public string Sku { get; set; }

    public string Keywords { get; set; }

    public bool? IsDeleted { get; set; } = false;
}
