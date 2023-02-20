using Volo.Abp.Application.Dtos;
using XCloud.Core.Helper;
using XCloud.Sales.Services.Catalog;
using XCloud.Sales.Services.Common;
using XCloud.Sales.Services.Coupons;
using XCloud.Sales.Services.Promotion;

namespace XCloud.Sales.ViewService;

public class SearchView : IEntityDto
{
    public string[] Keywords { get; set; }
    public TagDto[] Tags { get; set; }
    public BrandDto[] Brands { get; set; }
    public GoodsCollectionDto[] Collections { get; set; }
}

public class DashboardCountView : IEntityDto
{
    public int Goods { get; set; }
    public int Orders { get; set; }
    public int Brand { get; set; }
    public int Category { get; set; }
    public int Tag { get; set; }
    public int AfterSale { get; set; }
    public int ActiveUser { get; set; }
    public int Coupon { get; set; }
    public int Promotion { get; set; }
    public decimal Balance { get; set; }
    public decimal PrepaidCard { get; set; }
}

public class RefreshViewCacheInput : IEntityDto
{
    public bool Home { get; set; }
    public bool RootCategory { get; set; }
    public bool Dashboard { get; set; }
    public bool Search { get; set; }
    public bool MallSettings { get; set; }
}

public class HomePageCategoryAndGoodsDto : IEntityDto
{
    public CategoryDto Category { get; set; }
    public GoodsDto[] Goods { get; set; }
    public int Order { get; set; }
}

public class HomePageDto : IEntityDto
{
    public CouponDto[] Coupons { get; set; }
    public StorePromotionDto[] Promotions { get; set; }
    public PagesDto Topic { get; set; }
    public GoodsDto[] HotGoods { get; set; }
    public GoodsDto[] NewGoods { get; set; }
    public HomePageCategoryAndGoodsDto[] CategoryAndGoods { get; set; }

    public IEnumerable<GoodsDto> AllGoods()
    {
        var response = new List<GoodsDto>();

        if (ValidateHelper.IsNotEmptyCollection(this.HotGoods))
            response.AddRange(this.HotGoods);

        if (ValidateHelper.IsNotEmptyCollection(this.NewGoods))
            response.AddRange(this.NewGoods);

        if (ValidateHelper.IsNotEmptyCollection(this.CategoryAndGoods))
        {
            var categoryGoods = this.CategoryAndGoods.SelectMany(x => x.Goods).ToArray();
            response.AddRange(categoryGoods);
        }

        response = response.WhereNotNull().ToList();
        return response;
    }
}

public class CategoryPageDto : IEntityDto
{
    public CategoryDto[] Root { get; set; }
}