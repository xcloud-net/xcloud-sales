using XCloud.Sales.Core;

namespace XCloud.Sales.Data.Domain.Aftersale;

public class AfterSales : SalesBaseEntity<string>, IHasModificationTime, IHasCreationTime, ISoftDelete
{
    public string OrderId { get; set; }

    public int UserId { get; set; }

    public string ReasonForReturn { get; set; }

    public string RequestedAction { get; set; }

    public string UserComments { get; set; }

    public string StaffNotes { get; set; }

    public string Images { get; set; }

    public int AfterSalesStatusId { get; set; }

    public DateTime CreationTime { get; set; }

    public DateTime? LastModificationTime { get; set; }

    public bool HideForAdmin { get; set; }

    public bool IsDeleted { get; set; }
}