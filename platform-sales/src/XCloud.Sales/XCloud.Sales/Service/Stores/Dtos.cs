using Volo.Abp.Application.Dtos;
using XCloud.Core.Dto;
using XCloud.Sales.Data.Domain.Stores;
using XCloud.Sales.Service.Media;

namespace XCloud.Sales.Service.Stores;

public class StoreDto : Store, IEntityDto<string>
{
    public MallStorageMetaDto Picture { get; set; }
}

public class QueryStorePagingInput : PagedRequest
{
    //
}

public class SaveGoodsStoreMappingDto : IEntityDto
{
    public int GoodsCombinationId { get; set; }
    public string[] StoreIds { get; set; }
}

public class UpdateStoreStatusInput : IEntityDto
{
    public string StoreId { get; set; }
    public bool? IsDeleted { get; set; }
    public bool? IsActive { get; set; }
    public bool? IsClosed { get; set; }
}

public class UpdateStoreManagerStatusInput : IEntityDto
{
    public string ManagerId { get; set; }
    public bool? IsActive { get; set; }
    public bool? IsDeleted { get; set; }
}

public class StoreManagerDto : StoreManager
{
    //
}