using XCloud.Sales.Core;

namespace XCloud.Sales.Data.Domain.Stores;

public class Store : SalesBaseEntity<string>, IHasCreationTime, ISoftDelete
{
    public string StoreName { get; set; }

    public string StoreUrl { get; set; }

    public int StoreLogo { get; set; }

    public string CopyrightInfo { get; set; }

    public string ICPRecord { get; set; }

    public string StoreServiceTime { get; set; }

    public string ServiceTelePhone { get; set; }

    public bool StoreClosed { get; set; }

    public bool IsActive { get; set; }
    public bool IsDeleted { get; set; }

    public DateTime CreationTime { get; set; }
}