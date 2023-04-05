using Volo.Abp;
using Volo.Abp.Auditing;
using XCloud.Core.Application.Entity;
using XCloud.Platform.Core.Database;
using XCloud.Platform.Shared.Entity;

namespace XCloud.Platform.Core.Domain.Address;

public class UserAddress : EntityBase, ISoftDelete, IHasDeletionTime, IHasUserId, IPlatformEntity
{
    public string UserId { get; set; }

    public double? Lat { get; set; }
    public double? Lng { get; set; }

    public string Name { get; set; }

    public string NationCode { get; set; }
    public string Nation { get; set; }

    public string ProvinceCode { get; set; }
    public string Province { get; set; }

    public string CityCode { get; set; }
    public string City { get; set; }

    public string AreaCode { get; set; }
    public string Area { get; set; }

    public string AddressDescription { get; set; }

    public string AddressDetail { get; set; }

    public string PostalCode { get; set; }

    public string Tel { get; set; }

    public bool IsDefault { get; set; }

    public DateTime? DeletionTime { get; set; }
    public bool IsDeleted { get; set; }
}