using XCloud.Sales.Core;

namespace XCloud.Sales.Data.Domain.Catalog;

public class GoodsReview : SalesBaseEntity
{
    public int GoodsId { get; set; }

    public int UserId { get; set; }

    public string OrderId { get; set; }

    public string Title { get; set; }

    public string ReviewText { get; set; }

    public int Rating { get; set; }

    public string IpAddress { get; set; }

    public DateTime CreationTime { get; set; }
}