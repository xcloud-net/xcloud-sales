using Volo.Abp.Application.Dtos;
using XCloud.Core.Dto;
using XCloud.Sales.Data.Domain.Stock;
using XCloud.Sales.Services.Catalog;

namespace XCloud.Sales.Services.Stock;

public class SupplierDto : Supplier, IEntityDto
{
    //
}

public class WarehouseDto : Warehouse, IEntityDto
{
    //
}

public class UpdateWarehouseStatusInput : IEntityDto<string>
{
    public string Id { get; set; }
    public bool? IsDeleted { get; set; }
}

public class UpdateSupplierStatusInput : IEntityDto<string>
{
    public string Id { get; set; }
    public bool? IsDeleted { get; set; }
}

public class QueryWarehousePagingInput : PagedRequest
{
    //
}

public class QuerySupplierPagingInput : PagedRequest
{
    //
}

public class DeductStockInput : IEntityDto
{
    public int CombinationId { get; set; }
    public int Quantity { get; set; }
}

public class QueryWarehouseStockInput : PagedRequest, IEntityDto
{
    public string SeiralNo { get; set; }
    public string SupplierId { get; set; }
    public string WarehouseId { get; set; }
    public bool? Approved { get; set; }
    public DateTime? StartTime { get; set; }
    public DateTime? EndTime { get; set; }
}

public class QueryWarehouseStockItemInput : QueryWarehouseStockInput, IEntityDto
{
    public int? GoodsId { get; set; }
    public int? CombinationId { get; set; }
    public bool? RunningOut { get; set; }
}

public class WarehouseStockDto : WarehouseStock, IEntityDto
{
    public SupplierDto Supplier { get; set; }
    public WarehouseDto Warehouse { get; set; }
    public WarehouseStockItemDto[] Items { get; set; }
}

public class WarehouseStockItemDto : WarehouseStockItem, IEntityDto
{
    public WarehouseStockDto WarehouseStock { get; set; }
    public GoodsDto Goods { get; set; }
    public GoodsSpecCombinationDto Combination { get; set; }
}

public class AttachWarehouseStockDataInput : IEntityDto
{
    public bool Warehouse { get; set; }
    public bool Supplier { get; set; }
    public bool Items { get; set; }
}

public class AttachWarehouseStockItemDataInput : IEntityDto
{
    public bool Goods { get; set; }
    public bool WarehouseStock { get; set; }
}