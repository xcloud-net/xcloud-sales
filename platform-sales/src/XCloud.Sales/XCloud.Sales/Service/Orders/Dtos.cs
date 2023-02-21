using System.Text;
using Volo.Abp.Application.Dtos;
using XCloud.Core.Dto;
using XCloud.Core.Helper;
using XCloud.Sales.Data.Domain.Catalog;
using XCloud.Sales.Data.Domain.Orders;
using XCloud.Sales.Service.AfterSale;
using XCloud.Sales.Service.Catalog;
using XCloud.Sales.Service.Users;

namespace XCloud.Sales.Service.Orders;

/// <summary>
/// 库存扣件策略
/// </summary>
public enum OrderStockDeductStrategy : int
{
    AfterPlaceOrder = 1,
    AfterPayment = 2,
    AfterComplete = 3
}

public class OrderCanBeRefundResponse : ApiResponse<object>
{
    //
}

public class CreateOrderPayBillInput : IEntityDto<string>
{
    public string Id { get; set; }
    public decimal? Price { get; set; }
}

public class PlaceOrderResult : ApiResponse<Order>
{
    //
}

public class QueryPendingCountInput : IEntityDto
{
    public int? UserId { get; set; }
}

public class QueryAftersalePendingCountInput : IEntityDto
{
    public int? UserId { get; set; }
}

public class UpdateOrderInput : IEntityDto<string>
{
    public string Id { get; set; }
    public bool? HideForAdmin { get; set; }
    public bool? IsDeleted { get; set; }
}

public class MarkOrderAsPaidInput : IEntityDto
{
    public string OrderId { get; set; }
    public string Comment { get; set; }
}

public class UpdateOrderStatusInput : IEntityDto<string>
{
    public string Id { get; set; }
    public int? OrderStatus { get; set; }
    public int? PaymentStatus { get; set; }
    public int? DeliveryStatus { get; set; }
}

public class CancelOrderInput : IEntityDto
{
    public string OrderId { get; set; }
    public string Comment { get; set; }
}

public class CompleteOrderInput : IEntityDto
{
    public string OrderId { get; set; }
    public string Comment { get; set; }
}

public class PlaceOrderRequestDto : IEntityDto
{
    public StoreUserDto UserHolder { get; set; }

    public string StoreId { get; set; }

    public int UserId { get; set; }

    public int? CouponId { get; set; }

    public string PromotionId { get; set; }

    public string AddressId { get; set; }

    public string AddressContact { get; set; }

    public string AddressPhone { get; set; }

    public string AddressProvince { get; set; }

    public string AddressCity { get; set; }

    public string AddressArea { get; set; }

    public string AddressDetail { get; set; }

    public string Remark { get; set; }

    public PlaceOrderRequestItemDto[] Items { get; set; }

    public bool UseGradePrice { get; set; } = true;

    public string FingerPrint()
    {
        var sb = new StringBuilder();

        sb.Append($"user:{this.UserId}");
        if (ValidateHelper.IsNotEmptyCollection(this.Items))
        {
            var arr = Items.Select(x => x.FingerPrint()).OrderBy(x => x).ToArray();
            sb.Append($"items:{string.Join('-', arr)}");
        }

        return sb.ToString();
    }
}

public class PlaceOrderRequestItemDto : IEntityDto
{
    public int GoodsSpecCombinationId { get; set; }

    public int Quantity { get; set; }

    public string FingerPrint() => $"quatity:{Quantity}-specs:{this.GoodsSpecCombinationId}";
}

public class PlaceOrderCheckResponse : MultipleErrorsDto
{
    public PlaceOrderCheckResponse()
    {
        //
    }

    public PlaceOrderCheckResponse(PlaceOrderCheckInput input)
    {
        if (input == null)
            throw new ArgumentNullException(nameof(input));
        this.CheckInput = input;
    }

    public PlaceOrderCheckInput CheckInput { get; set; }
}

public class PlaceOrderCheckInput : IEntityDto
{
    public int GoodsSpecCombinationId { get; set; }
    public int Quantity { get; set; }
    public string StoreId { get; set; }
}

public class OrderItemDto : OrderItem, IEntityDto
{
    public GoodsDto Goods { get; set; }
    public GoodsSpecCombination GoodsSpecCombination { get; set; }
}

public class OrderNoteDto : OrderNote, IEntityDto
{
    //
}

public class QueryOrderNotesInput : IEntityDto
{
    public string OrderId { get; set; }
    public bool OnlyForUser { get; set; } = false;
    public int? MaxCount { get; set; }
}

public class OrderDto : Order, IEntityDto
{
    public string StoreName { get; set; }

    public StoreUserDto User { get; set; }

    public OrderItemDto[] Items { get; set; }

    public AfterSalesDto AfterSales { get; set; }

    public OrderStockDeductStrategy OrderStockDeductStrategy { get; set; }
}

public class OrderItemAttachDataInput : IEntityDto
{
    public bool Goods { get; set; } = false;
}

public class OrderAttachDataOption : IEntityDto
{
    public bool User { get; set; } = false;
    public bool OrderItems { get; set; } = false;
    public bool Store { get; set; } = false;
    public bool AfterSales { get; set; } = false;
    public bool Address { get; set; } = false;
    public bool Coupon { get; set; } = false;
    public bool Activity { get; set; }
}

public class QueryOrderInput : PagedRequest
{
    public string Sn { get; set; }
    public int? AffiliateId { get; set; }
    public string StoreId { get; set; }

    public bool? IsAfterSales { get; set; }

    public bool? IsAfterSalesPending { get; set; }

    public int[] AfterSalesStatus { get; set; }

    public DateTime? StartTime { get; set; }
    public DateTime? EndTime { get; set; }
    public int[] Status { get; set; }
    public int? PaymentStatus { get; set; }
    public int? ShippingStatus { get; set; }
    public int? UserId { get; set; }
    public bool? IsDeleted { get; set; }
    public bool? HideForAdmin { get; set; }

    public bool? SortForAdmin { get; set; }
}