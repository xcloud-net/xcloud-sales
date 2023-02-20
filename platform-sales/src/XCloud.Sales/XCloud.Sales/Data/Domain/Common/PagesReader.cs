using XCloud.Sales.Core;

namespace XCloud.Sales.Data.Domain.Common;

public class PagesReader : SalesBaseEntity, IHasCreationTime
{
    public string PageId { get; set; }

    public int UserId { get; set; }

    public DateTime CreationTime { get; set; }
}