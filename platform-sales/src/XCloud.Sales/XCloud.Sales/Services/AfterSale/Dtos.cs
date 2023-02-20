using Volo.Abp.Application.Dtos;
using XCloud.Core.Dto;
using XCloud.Sales.Data.Domain.Aftersale;
using XCloud.Sales.Services.Media;
using XCloud.Sales.Services.Orders;
using XCloud.Sales.Services.Users;

namespace XCloud.Sales.Services.AfterSale;

public class AfterSalesCommentDto : AfterSalesComment, IEntityDto
{
    public AfterSalesDto AfterSales { get; set; }
    
    public MallStorageMetaDto[] Pictures { get; set; }
}

public class QueryAfterSalesCommentPagingInput : PagedRequest
{
    public string AfterSalesId { get; set; }
}

public class AttachDataInput : IEntityDto
{
    public bool Items { get; set; } = false;
    public bool User { get; set; } = false;
    public bool Order { get; set; } = false;
}

public class AttachAftersalesItemsDataInput : IEntityDto
{
    public bool OrderItems { get; set; } = false;
}

public class DangerouslyUpdateAfterSalesStatusInput : IEntityDto<string>
{
    public string Id { get; set; }
    public int? Status { get; set; }
}

public class ApproveAftersaleInput : IEntityDto<string>
{
    public string Id { get; set; }
    public string Comment { get; set; }
}

public class RejectAftersaleInput : IEntityDto<string>
{
    public string Id { get; set; }
    public string Comment { get; set; }
}

public class CancelAftersaleInput : IEntityDto<string>
{
    public string Id { get; set; }
    public string Comment { get; set; }
}

public class UpdateAfterSaleStatusInput : IEntityDto<string>
{
    public string Id { get; set; }
    public bool? IsDeleted { get; set; }
    public bool? HideForAdmin { get; set; }
}

public class CompleteAftersaleInput : IEntityDto<string>
{
    public string Id { get; set; }
    public string Comment { get; set; }
}

public class AfterSalesDto : AfterSales, IEntityDto
{
    public StoreUserDto User { get; set; }

    public AfterSalesItemDto[] Items { get; set; }

    public OrderDto Order { get; set; }

    public AfterSalesStatus AfterSalesStatus
    {
        get => (AfterSalesStatus)this.AfterSalesStatusId;
        set => this.AfterSalesStatusId = (int)value;
    }
}

public class AfterSalesItemDto : AftersalesItem, IEntityDto
{
    public OrderItemDto OrderItem { get; set; }
}

public class QueryAfterSaleInput : PagedRequest
{
    public int? UserId { get; set; }
    public int[] Status { get; set; }
    public DateTime? StartTime { get; set; }
    public DateTime? EndTime { get; set; }
    public bool? IsDeleted { get; set; }

    public bool? IsAfterSalesPending { get; set; }

    public bool? HideForAdmin { get; set; }

    public bool? SortForAdmin { get; set; }
}