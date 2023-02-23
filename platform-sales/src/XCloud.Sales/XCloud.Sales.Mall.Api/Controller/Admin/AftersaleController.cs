using Microsoft.AspNetCore.Mvc;
using XCloud.Core.Dto;
using XCloud.Sales.Service.AfterSale;
using XCloud.Sales.Service.Authentication;
using XCloud.Sales.Service.Catalog;
using XCloud.Sales.Service.Orders;

namespace XCloud.Sales.Mall.Api.Controller.Admin;

[StoreAuditLog]
[Route("api/mall-admin/aftersale")]
public class AfterSaleController : ShopBaseController
{
    private readonly IOrderService _orderService;
    private readonly IAfterSaleService _aftersaleService;
    private readonly IAfterSaleCommentService _afterSaleCommentService;
    private readonly IGoodsService _goodsService;

    public AfterSaleController(IOrderService orderService,
        IGoodsService goodsService,
        IAfterSaleService aftersaleService, 
        IAfterSaleCommentService afterSaleCommentService)
    {
        this._goodsService = goodsService;
        this._aftersaleService = aftersaleService;
        _afterSaleCommentService = afterSaleCommentService;
        this._orderService = orderService;
    }
    
    [HttpPost("add-comment")]
    public async Task<ApiResponse<object>> InsertCommentAsync([FromBody] AfterSalesCommentDto dto)
    {
        var storeAdministrator = await this.StoreAuthService.GetRequiredStoreAdministratorAsync();

        await this.SalesPermissionService.CheckRequiredPermissionAsync(storeAdministrator,
            SalesPermissions.ManageAfterSales);

        dto.IsAdmin = true;

        await this._afterSaleCommentService.InsertAsync(dto);

        return new ApiResponse<object>();
    }

    [HttpPost("comment-paging")]
    public async Task<PagedResponse<AfterSalesCommentDto>> QueryCommentPagingAsync(
        [FromBody] QueryAfterSalesCommentPagingInput dto)
    {
        var storeAdministrator = await this.StoreAuthService.GetRequiredStoreAdministratorAsync();

        await this.SalesPermissionService.CheckRequiredPermissionAsync(storeAdministrator,
            SalesPermissions.ManageAfterSales);

        var response = await this._afterSaleCommentService.QueryPagingAsync(dto);

        return response;
    }

    [HttpPost("update-status")]
    public async Task<ApiResponse<object>> UpdateStatusAsync([FromBody] UpdateAfterSaleStatusInput dto)
    {
        var storeAdministrator = await this.StoreAuthService.GetRequiredStoreAdministratorAsync();

        await this.SalesPermissionService.CheckRequiredPermissionAsync(storeAdministrator,
            SalesPermissions.ManageAfterSales);

        await this._aftersaleService.UpdateStatusAsync(dto);

        return new ApiResponse<object>();
    }

    [HttpPost("dangerously-update-status")]
    public async Task<ApiResponse<object>> UpdateAftersalesStatusAsync(
        [FromBody] DangerouslyUpdateAfterSalesStatusInput dto)
    {
        var storeAdministrator = await this.StoreAuthService.GetRequiredStoreAdministratorAsync();

        await this.SalesPermissionService.CheckRequiredPermissionAsync(storeAdministrator,
            SalesPermissions.ManageAfterSales);

        await this._aftersaleService.DangerouslyUpdateStatusAsync(dto);

        return new ApiResponse<object>();
    }

    [HttpPost("cancel")]
    public async Task<ApiResponse<object>> CancelAsync([FromBody] CancelAftersaleInput dto)
    {
        var storeAdministrator = await this.StoreAuthService.GetRequiredStoreAdministratorAsync();

        await this.SalesPermissionService.CheckRequiredPermissionAsync(storeAdministrator,
            SalesPermissions.ManageAfterSales);

        await this._aftersaleService.CancelAsync(dto);

        return new ApiResponse<object>();
    }

    [HttpPost("complete")]
    public async Task<ApiResponse<object>> CompleteAsync([FromBody] CompleteAftersaleInput dto)
    {
        var storeAdministrator = await this.StoreAuthService.GetRequiredStoreAdministratorAsync();

        await this.SalesPermissionService.CheckRequiredPermissionAsync(storeAdministrator,
            SalesPermissions.ManageAfterSales);

        await this._aftersaleService.CompleteAsync(dto);

        return new ApiResponse<object>();
    }

    [HttpPost("approve")]
    public async Task<ApiResponse<object>> ApproveAsync([FromBody] ApproveAftersaleInput dto)
    {
        var storeAdministrator = await this.StoreAuthService.GetRequiredStoreAdministratorAsync();

        await this.SalesPermissionService.CheckRequiredPermissionAsync(storeAdministrator,
            SalesPermissions.ManageAfterSales);

        await this._aftersaleService.ApproveAsync(dto);

        return new ApiResponse<object>();
    }

    [HttpPost("reject")]
    public async Task<ApiResponse<object>> RejectAsync([FromBody] RejectAftersaleInput dto)
    {
        var storeAdministrator = await this.StoreAuthService.GetRequiredStoreAdministratorAsync();

        await this.SalesPermissionService.CheckRequiredPermissionAsync(storeAdministrator,
            SalesPermissions.ManageAfterSales);

        await this._aftersaleService.RejectAsync(dto);

        return new ApiResponse<object>();
    }

    [NonAction]
    private async Task AttachDataAsync(AfterSalesDto[] data)
    {
        if (!data.Any())
            return;

        await this._aftersaleService.AttachDataAsync(data,
            new AttachDataInput() { Order = true, Items = true, User = true });

        var mallUsers = data.Select(x => x.User).WhereNotNull().ToArray();
        await this.PlatformInternalService.AttachSysUserAsync(mallUsers);

        var aftersalesItems = data.SelectMany(x => x.Items).ToArray();

        if (!aftersalesItems.Any())
            return;

        await this._aftersaleService.AttachAfterSalesItemsDataAsync(aftersalesItems,
            new AttachAftersalesItemsDataInput() { OrderItems = true });

        var orderItems = aftersalesItems.Select(x => x.OrderItem).WhereNotNull().ToArray();

        if (!orderItems.Any())
            return;

        await this._orderService.AttachOrderItemDataAsync(orderItems,
            new OrderItemAttachDataInput() { Goods = true });

        var goods = orderItems.Select(x => x.Goods).WhereNotNull().ToArray();

        if (!goods.Any())
            return;

        await this._goodsService.AttachDataAsync(goods, new AttachGoodsDataInput() { Images = true });
    }

    [HttpPost("by-id")]
    public async Task<ApiResponse<AfterSalesDto>> QueryByIdAsync([FromBody] IdDto dto)
    {
        var storeAdministrator = await this.StoreAuthService.GetRequiredStoreAdministratorAsync();

        await this.SalesPermissionService.CheckRequiredPermissionAsync(storeAdministrator,
            SalesPermissions.ManageAfterSales);

        var aftersalesDto = await this._aftersaleService.QueryByIdAsync(dto.Id);

        if (aftersalesDto == null)
            throw new EntityNotFoundException();

        await this.AttachDataAsync(new[] { aftersalesDto });

        return new ApiResponse<AfterSalesDto>(aftersalesDto);
    }

    [HttpPost("paging")]
    public async Task<PagedResponse<AfterSalesDto>> PagingAsync([FromBody] QueryAfterSaleInput dto)
    {
        var storeAdministrator = await this.StoreAuthService.GetRequiredStoreAdministratorAsync();

        await this.SalesPermissionService.CheckRequiredPermissionAsync(storeAdministrator,
            SalesPermissions.ManageAfterSales);

        dto.PageSize = 20;
        dto.IsDeleted = null;
        dto.HideForAdmin = null;
        dto.SortForAdmin = true;

        var response = await this._aftersaleService.QueryPagingAsync(dto);

        await this.AttachDataAsync(response.Items.ToArray());

        return response;
    }
}