using XCloud.Sales.Core;

namespace XCloud.Sales.Data.Domain.Aftersale;

public class AfterSalesComment : SalesBaseEntity<string>, IHasCreationTime
{
    public string AfterSaleId { get; set; }
    public string Content { get; set; }
    public string PictureJson { get; set; }
    public bool IsAdmin { get; set; }
    public DateTime CreationTime { get; set; }
}