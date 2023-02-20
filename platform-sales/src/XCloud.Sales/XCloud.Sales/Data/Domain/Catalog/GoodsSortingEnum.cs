namespace XCloud.Sales.Data.Domain.Catalog;

public enum GoodsSortingEnum : int
{
    /// <summary>
    /// 价格: Low to High
    /// </summary>
    PriceAsc = 10,
    /// <summary>
    /// 价格: High to Low
    /// </summary>
    PriceDesc = 11,
    /// <summary>
    /// 创建时间 creation date
    /// </summary>
    CreationTime = 15,
    /// <summary>
    /// 按销量升序
    /// </summary>
    SaleVolumeAsc = 20,
    /// <summary>
    /// 按销量降序
    /// </summary>
    SaleVolumeDesc = 21,
}