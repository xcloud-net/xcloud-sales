using Microsoft.AspNetCore.Mvc;
using XCloud.Core.Dto;
using XCloud.Sales.Data.Domain.Orders;
using XCloud.Sales.Services.Finance;
using XCloud.Sales.Services.Orders;

namespace XCloud.Sales.Mall.Api.Controllers.Admin;

[StoreAuditLog]
[Route("api/mall-admin/order")]
public class OrderController : ShopBaseController
{
    private readonly IOrderService _orderService;
    private readonly IOrderProcessingService _orderProcessingService;
    private readonly IOrderBillService _orderBillService;

    public OrderController(IOrderService orderService,
        IOrderProcessingService orderProcessingService,
        IOrderBillService orderBillService)
    {
        this._orderService = orderService;
        this._orderProcessingService = orderProcessingService;
        this._orderBillService = orderBillService;
    }

    [HttpPost("add-order-note")]
    public async Task<ApiResponse<object>> AddOrderNoteAsync([FromBody] OrderNoteDto dto)
    {
        var storeAdministrator = await this.StoreAuthService.GetRequiredStoreAdministratorAsync();

        await this.SalesPermissionService.CheckRequiredPermissionAsync(storeAdministrator,
            SalesPermissions.ManageOrders);

        var entity = this.ObjectMapper.Map<OrderNoteDto, OrderNote>(dto);

        await this._orderService.InsertOrderNoteAsync(entity);

        return new ApiResponse<object>();
    }

    [HttpPost("update-status")]
    public async Task<ApiResponse<object>> UpdateStatusAsync([FromBody] UpdateOrderInput dto)
    {
        var storeAdministrator = await this.StoreAuthService.GetRequiredStoreAdministratorAsync();

        await this.SalesPermissionService.CheckRequiredPermissionAsync(storeAdministrator,
            SalesPermissions.ManageOrders);

        await this._orderService.UpdateStatusAsync(dto);

        return new ApiResponse<object>();
    }

    [HttpPost("dangerously-update-status")]
    public async Task<ApiResponse<object>> DangerouslyUpdateStatusAsync([FromBody] UpdateOrderStatusInput dto)
    {
        var storeAdministrator = await this.StoreAuthService.GetRequiredStoreAdministratorAsync();

        await this.SalesPermissionService.CheckRequiredPermissionAsync(storeAdministrator,
            SalesPermissions.ManageOrders);

        await this._orderProcessingService.DangerouslyUpdateStatusAsync(dto);

        return new ApiResponse<object>();
    }

    [HttpPost("list-order-notes")]
    public async Task<ApiResponse<OrderNoteDto[]>> QueryOrderNotesAsync([FromBody] QueryOrderNotesInput dto)
    {
        var storeAdministrator = await this.StoreAuthService.GetRequiredStoreAdministratorAsync();

        await this.SalesPermissionService.CheckRequiredPermissionAsync(storeAdministrator,
            SalesPermissions.ManageOrders);

        var notes = await this._orderService.QueryOrderNotesAsync(dto);

        return new ApiResponse<OrderNoteDto[]>(notes);
    }

    [HttpPost("create-order-bill")]
    public async Task<ApiResponse<OrderBillDto>> CreateOrderBillAsync([FromBody] IdDto dto)
    {
        var storeAdministrator = await this.StoreAuthService.GetRequiredStoreAdministratorAsync();

        await this.SalesPermissionService.CheckRequiredPermissionAsync(storeAdministrator,
            SalesPermissions.ManageOrders);

        var order = await this._orderService.QueryByIdAsync(dto.Id);
        if (order == null)
            throw new EntityNotFoundException();

        var bill = await this._orderProcessingService.CreateOrderPayBillAsync(new CreateOrderPayBillInput()
            { Id = order.Id });

        return new ApiResponse<OrderBillDto>(bill);
    }

    [HttpPost("mark-bill-as-paid")]
    public async Task<ApiResponse<object>> MarkBillAsPaid([FromBody] MarkBillAsPayedInput dto)
    {
        var storeAdministrator = await this.StoreAuthService.GetRequiredStoreAdministratorAsync();

        await this.SalesPermissionService.CheckRequiredPermissionAsync(storeAdministrator,
            SalesPermissions.ManageOrders);

        var bill = await this._orderBillService.QueryByIdAsync(dto.Id);
        if (bill == null)
            throw new EntityNotFoundException();

        dto.PaymentMethod = (int)PaymentMethod.Manual;

        await this._orderBillService.MarkBillAsPayedAsync(dto);
        //commit to ensure next step can read the latest data

        //trigger order payment status updation
        await this._orderProcessingService.TrySetAsPaidAfterBillPaidAsync(new IdDto() { Id = bill.OrderId });

        return new ApiResponse<object>();
    }

    [Obsolete]
    [HttpPost("try-mark-order-as-paid")]
    public async Task<ApiResponse<object>> TryMarkOrderAsPaid([FromBody] IdDto dto)
    {
        var storeAdministrator = await this.StoreAuthService.GetRequiredStoreAdministratorAsync();

        await this.SalesPermissionService.CheckRequiredPermissionAsync(storeAdministrator,
            SalesPermissions.ManageOrders);

        //trigger order payment status updation
        await this._orderProcessingService.TrySetAsPaidAfterBillPaidAsync(dto);

        return new ApiResponse<object>();
    }

    [HttpPost("list-order-bill")]
    public async Task<ApiResponse<OrderBillDto[]>> ListOrderBillAsync([FromBody] IdDto dto)
    {
        var storeAdministrator = await this.StoreAuthService.GetRequiredStoreAdministratorAsync();

        await this.SalesPermissionService.CheckRequiredPermissionAsync(storeAdministrator,
            SalesPermissions.ManageOrders);

        var bills = await this._orderBillService.QueryByOrderIdAsync(new ListOrderBillInput()
            { Id = dto.Id });

        return new ApiResponse<OrderBillDto[]>(bills);
    }

    [HttpPost("paging")]
    public async Task<PagedResponse<OrderDto>> OrderList([FromBody] QueryOrderInput dto)
    {
        var storeAdministrator = await this.StoreAuthService.GetRequiredStoreAdministratorAsync();

        await this.SalesPermissionService.CheckRequiredPermissionAsync(storeAdministrator,
            SalesPermissions.ManageOrders);

        dto.PageSize = 20;
        dto.SortForAdmin = true;
        dto.IsDeleted = null;
        dto.HideForAdmin = null;

        var res = await this._orderService.QueryPagingAsync(dto);

        return res;
    }

    /// <summary>
    /// 获取订单详情
    /// </summary>
    [HttpPost("detail")]
    public async Task<ApiResponse<OrderDto>> Detail([FromBody] IdDto dto)
    {
        var storeAdministrator = await this.StoreAuthService.GetRequiredStoreAdministratorAsync();

        await this.SalesPermissionService.CheckRequiredPermissionAsync(storeAdministrator,
            SalesPermissions.ManageOrders);

        var order = await this._orderService.QueryDetailByIdAsync(dto.Id);

        if (order == null)
            throw new EntityNotFoundException();

        return new ApiResponse<OrderDto>(order);
    }

    [HttpPost("cancel")]
    public async Task<ApiResponse<object>> CancelOrder([FromBody] CancelOrderInput dto)
    {
        var storeAdministrator = await this.StoreAuthService.GetRequiredStoreAdministratorAsync();

        await this.SalesPermissionService.CheckRequiredPermissionAsync(storeAdministrator,
            SalesPermissions.ManageOrders);

        await _orderProcessingService.CancelAsync(dto);

        return new ApiResponse<object>();
    }

    [HttpPost("hide")]
    public async Task<ApiResponse<object>> Hide([FromBody] IdDto<string> dto)
    {
        var storeAdministrator = await this.StoreAuthService.GetRequiredStoreAdministratorAsync();

        await this.SalesPermissionService.CheckRequiredPermissionAsync(storeAdministrator,
            SalesPermissions.ManageOrders);

        await this._orderService.UpdateStatusAsync(new UpdateOrderInput()
            { Id = dto.Id, HideForAdmin = true });

        return new ApiResponse<object>();
    }
}