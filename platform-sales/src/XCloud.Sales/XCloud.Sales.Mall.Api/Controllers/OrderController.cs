using Microsoft.AspNetCore.Mvc;
using XCloud.Core.Dto;
using XCloud.Sales.Data.Domain.Logging;
using XCloud.Sales.Service.Authentication;
using XCloud.Sales.Service.Finance;
using XCloud.Sales.Service.Logging;
using XCloud.Sales.Service.Orders;

namespace XCloud.Sales.Mall.Api.Controllers;

/// <summary>
/// 订单接口
/// </summary>
[Route("api/mall/order")]
public class OrderController : ShopBaseController
{
    private readonly IOrderService _orderService;
    private readonly IOrderProcessingService _orderProcessingService;
    private readonly IPlaceOrderService _placeOrderService;
    private readonly IOrderBillService _orderBillService;

    /// <summary>
    /// 构造器
    /// </summary>
    public OrderController(
        IOrderService orderService,
        IOrderBillService orderBillService,
        IPlaceOrderService placeOrderService,
        IOrderProcessingService orderProcessingService)
    {
        this._placeOrderService = placeOrderService;
        this._orderBillService = orderBillService;
        this._orderService = orderService;
        this._orderProcessingService = orderProcessingService;
    }

    [HttpPost("pending-count")]
    public async Task<ApiResponse<int>> PendingCountAsync()
    {
        var loginUser = await this.StoreAuthService.GetRequiredStoreUserAsync();

        var count = await this._orderService.QueryPendingCountAsync(new QueryPendingCountInput()
            { UserId = loginUser.Id });

        return new ApiResponse<int>(count);
    }

    /// <summary>
    /// 提交订单
    /// </summary>
    [HttpPost("place-order")]
    public async Task<PlaceOrderResult> PlaceOrderAsync([FromBody] PlaceOrderRequestDto dto)
    {
        var loginUser = await this.StoreAuthService.GetRequiredStoreUserAsync();

        dto.UserId = loginUser.Id;

        var result = await _placeOrderService.PlaceOrderAsync(dto);

        if (result.IsSuccess())
            await this.EventBusService.NotifyInsertActivityLog(new ActivityLog()
            {
                ActivityLogTypeId = (int)ActivityLogType.PlaceOrder,
                UserId = loginUser.Id,
                SubjectId = result.Data.Id,
                SubjectType = ActivityLogSubjectType.Order,
                Comment = "创建订单"
            });

        return result;
    }

    [HttpPost("paging")]
    public async Task<PagedResponse<OrderDto>> QueryOrderListAsync([FromBody] QueryOrderInput dto)
    {
        var loginUser = await this.StoreAuthService.GetRequiredStoreUserAsync();
            
        dto.UserId = loginUser.Id;
        dto.IsDeleted = false;
        dto.SortForAdmin = false;
        dto.HideForAdmin = null;

        var res = await this._orderService.QueryPagingAsync(dto);

        return res;
    }

    /// <summary>
    /// 获取订单详情
    /// </summary>
    [HttpPost("detail")]
    public async Task<ApiResponse<OrderDto>> DetailAsync([FromBody] IdDto dto)
    {
        var loginUser = await this.StoreAuthService.GetRequiredStoreUserAsync();

        var order = await this._orderService.QueryDetailByIdAsync(dto.Id);

        if (order == null || order.UserId != loginUser.Id)
            throw new EntityNotFoundException(nameof(order));

        return new ApiResponse<OrderDto>(order);
    }

    [HttpPost("list-order-bill")]
    public async Task<ApiResponse<OrderBillDto[]>> ListOrderBillAsync([FromBody] IdDto dto)
    {
        var loginUser = await this.StoreAuthService.GetRequiredStoreUserAsync();

        var order = await this._orderService.QueryByIdAsync(dto.Id);

        if (order == null || order.UserId != loginUser.Id)
            throw new EntityNotFoundException(nameof(order));

        var bills = await this._orderBillService.QueryByOrderIdAsync(new ListOrderBillInput()
        {
            Id = dto.Id,
            Paid = true
        });

        return new ApiResponse<OrderBillDto[]>(bills);
    }

    [HttpPost("list-order-notes")]
    public async Task<ApiResponse<OrderNoteDto[]>> QueryOrderNotesAsync([FromBody] QueryOrderNotesInput dto)
    {
        var loginUser = await this.StoreAuthService.GetRequiredStoreUserAsync();

        var order = await this._orderService.QueryByIdAsync(dto.OrderId);

        if (order == null || order.UserId != loginUser.Id)
            throw new EntityNotFoundException();

        dto.OnlyForUser = true;
            
        var notes = await this._orderService.QueryOrderNotesAsync(dto);

        return new ApiResponse<OrderNoteDto[]>(notes);
    }

    [HttpPost("complete")]
    public async Task<ApiResponse<object>> CompleteAsync([FromBody] CompleteOrderInput dto)
    {
        var loginUser = await this.StoreAuthService.GetRequiredStoreUserAsync();

        var order = await _orderService.QueryByIdAsync(dto.OrderId);
        if (order == null || loginUser.Id != order.UserId)
            throw new EntityNotFoundException(nameof(order));

        await _orderProcessingService.CompleteAsync(dto);

        return new ApiResponse<object>();
    }

    /// <summary>
    /// 取消订单
    /// </summary>
    [HttpPost("cancel")]
    public async Task<ApiResponse<object>> CancelOrderAsync([FromBody] CancelOrderInput dto)
    {
        var loginUser = await this.StoreAuthService.GetRequiredStoreUserAsync();

        var order = await _orderService.QueryByIdAsync(dto.OrderId);

        if (order == null || loginUser.Id != order.UserId)
            throw new EntityNotFoundException(nameof(order));

        await _orderProcessingService.CancelAsync(dto);

        return new ApiResponse<object>();
    }

    [HttpPost("delete")]
    public async Task<ApiResponse<object>> DeleteAsync([FromBody] IdDto<string> dto)
    {
        var loginUser = await this.StoreAuthService.GetRequiredStoreUserAsync();

        var order = await _orderService.QueryByIdAsync(dto.Id);
        if (order == null || order.UserId != loginUser.Id)
            throw new EntityNotFoundException(nameof(order));

        await this._orderService.UpdateStatusAsync(new UpdateOrderInput() { Id = order.Id, IsDeleted = true });

        return new ApiResponse<object>();
    }
}