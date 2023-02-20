using XCloud.Sales.Core;

namespace XCloud.Sales.Data.Domain.Stock;

public class WarehouseStock : SalesBaseEntity<string>, IHasCreationTime
{
    public string No { get; set; }

    public string SupplierId { get; set; }

    public string WarehouseId { get; set; }

    public string Remark { get; set; }

    public bool Approved { get; set; }
    public string ApprovedByUserId { get; set; }
    public DateTime? ApprovedTime { get; set; }

    [Obsolete]
    public DateTime ExpirationTime { get; set; }

    public DateTime CreationTime { get; set; }
}