using XCloud.Sales.Core;

namespace XCloud.Sales.Data.Domain.Catalog;

public class Goods : SalesBaseEntity, ISoftDelete
{
    public string Name { get; set; }

    public bool StickyTop { get; set; }

    public bool IsNew { get; set; }

    public bool IsHot { get; set; }

    public string SeoName { get; set; }

    public string ShortDescription { get; set; }

    public string Keywords { get; set; }

    public string FullDescription { get; set; }

    public string AdminComment { get; set; }

    public int AttributeType { get; set; }

    public int StockQuantity { get; set; }

    public int SaleVolume { get; set; }

    public int MaxAmountInOnePurchase { get; set; }

    public decimal CostPrice { get; set; }

    public decimal Price { get; set; }

    public decimal MinPrice { get; set; }

    public decimal MaxPrice { get; set; }

    public int CategoryId { get; set; }

    public int BrandId { get; set; }

    public double ApprovedReviewsRates { get; set; }

    public int ApprovedTotalReviews { get; set; }

    public bool Published { get; set; }

    public bool IsDeleted { get; set; }

    public DateTime CreationTime { get; set; }

    public DateTime LastModificationTime { get; set; }

}