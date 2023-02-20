using Volo.Abp.Application.Dtos;
using XCloud.Sales.Services.Catalog;
using XCloud.Sales.Services.Users;

namespace XCloud.Sales.Services.Report;

public class TopAfterSaleMallUsersResponse : IEntityDto
{
    public StoreUserDto User { get; set; }

    public int UserId { get; set; }
    public int Count { get; set; }
    public decimal Amount { get; set; }
}

public class TopAfterSaleMallUsersInput : IEntityDto
{
    public DateTime? StartTime { get; set; }
    public DateTime? EndTime { get; set; }
    public int? MaxCount { get; set; }
}

public class UserActivityGroupByGeoLocationInput : IEntityDto
{
    public int? UserId { get; set; }
    public DateTime? StartTime { get; set; }
    public DateTime? EndTime { get; set; }
        
    public int? MaxCount { get; set; }
}
    
public class UserActivityGroupByGeoLocationResponse : IEntityDto
{
    public string Country { get; set; }
    public string City { get; set; }
    public int Count { get; set; }
}    
    
public class UserActivityGroupByHourInput : IEntityDto
{
    public int? UserId { get; set; }
    public DateTime? StartTime { get; set; }
    public DateTime? EndTime { get; set; }
}

public class UserActivityGroupByHourResponse : IEntityDto
{
    public int Hour { get; set; }
    public int ActivityType { get; set; }
    public int Count { get; set; }
}

public class QueryGoodsVisitReportResponse : IEntityDto
{
    public GoodsDto Goods { get; set; }
    public int GoodsId { get; set; }
    public int VisitedCount { get; set; }
}

public class QuerySearchKeywordsReportResponse : IEntityDto
{
    public string Keywords { get; set; }
    public int Count { get; set; }
}

public class QuerySearchKeywordsReportInput : IEntityDto
{
    public int? UserId { get; set; }
    public DateTime? StartTime { get; set; }
    public DateTime? EndTime { get; set; }
    public int? MaxCount { get; set; }
}

public class QueryGoodsVisitReportInput : IEntityDto
{
    public int? UserId { get; set; }
    public int? GoodsId { get; set; }
    public DateTime? StartTime { get; set; }
    public DateTime? EndTime { get; set; }
    public int? Count { get; set; }
}

public class PriceHistoryResponse : IEntityDto
{
    public DateTime Date { get; set; }
    public decimal Price { get; set; }
    public bool? NoData { get; set; }
}

public class QueryCombinationPriceHistoryInput : IEntityDto<int>
{
    public int Id { get; set; }
    public DateTime? StartTime { get; set; }
    public DateTime? EndTime { get; set; }
    public int? LastDays { get; set; }
    public int? MaxTake { get; set; }
}

public class BaseOrderReportInput : IEntityDto
{
    public DateTime? StartTime { get; set; }
    public DateTime? EndTime { get; set; }
    public int? MaxCount { get; set; }
}

public class QueryTopSellerInput : BaseOrderReportInput
{
    //
}

public class TopSellersList : IEntityDto
{
    public int SellerId { get; set; }
    public string GlobalUserId { get; set; }
    public string SellerName { get; set; }
    public decimal TotalPrice { get; set; }
    public int TotalQuantity { get; set; }
}

public class QueryTopCustomerInput : BaseOrderReportInput
{
    //
}

public class TopCustomersList : IEntityDto
{
    public int CustomerId { get; set; }
    public string GlobalUserId { get; set; }
    public string CustomerName { get; set; }
    public decimal TotalPrice { get; set; }
    public int TotalQuantity { get; set; }
}

public class QueryTopSkuListInput : BaseOrderReportInput
{
    //
}

public class TopSkuList : IEntityDto
{
    public int SkuId { get; set; }
    public string Name { get; set; }
    public decimal TotalPrice { get; set; }
    public int TotalQuantity { get; set; }
}

public class QueryTopBrandListInput : BaseOrderReportInput
{
    //
}

public class TopBrandList : IEntityDto
{
    public int BrandId { get; set; }
    public string BrandName { get; set; }
    public decimal TotalPrice { get; set; }
    public int TotalQuantity { get; set; }
}

public class QueryTopCategoryListInput : BaseOrderReportInput
{
    //
}

public class TopCategoryList : IEntityDto
{
    public int RootCategoryId { get; set; }
    public string RootCategoryName { get; set; }
    public int CategoryId { get; set; }
    public string CategoryName { get; set; }
    public decimal TotalPrice { get; set; }
    public int TotalQuantity { get; set; }
}

public class OrderSumByDateInput : BaseOrderReportInput
{
    //
}

public class OrderSumByDateResponse : IEntityDto
{
    public DateTime Date { get; set; }
    public int Total { get; set; }
    public decimal Amount { get; set; }
}