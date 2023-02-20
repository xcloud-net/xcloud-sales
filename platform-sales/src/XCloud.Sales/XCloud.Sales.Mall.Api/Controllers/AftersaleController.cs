using Microsoft.AspNetCore.Mvc;
using XCloud.Core.Dto;
using XCloud.Sales.Services.AfterSale;
using XCloud.Sales.Services.Catalog;
using XCloud.Sales.Services.Orders;

namespace XCloud.Sales.Mall.Api.Controllers;

[Route("api/mall/aftersale")]
public class AfterSaleController : ShopBaseController
{
    private readonly IOrderService _orderService;
    private readonly IAfterSaleService _aftersaleService;
    private readonly IAfterSalesCommentService _afterSalesCommentService;
    private readonly IGoodsService _goodsService;

    public AfterSaleController(IOrderService orderService,
        IAfterSaleService aftersaleService,
        IGoodsService goodsService,
        IAfterSalesCommentService afterSalesCommentService)
    {
        this._aftersaleService = aftersaleService;
        this._orderService = orderService;
        this._goodsService = goodsService;
        _afterSalesCommentService = afterSalesCommentService;
    }

    [HttpPost("add-comment")]
    public async Task<ApiResponse<object>> InsertCommentAsync([FromBody] AfterSalesCommentDto dto)
    {
        var loginUser = await this.StoreAuthService.GetRequiredStoreUserAsync();

        dto.IsAdmin = false;

        await this._afterSalesCommentService.InsertAsync(dto);

        return new ApiResponse<object>();
    }

    [HttpPost("comment-paging")]
    public async Task<PagedResponse<AfterSalesCommentDto>> QueryCommentPagingAsync(
        [FromBody] QueryAfterSalesCommentPagingInput dto)
    {
        var loginUser = await this.StoreAuthService.GetRequiredStoreUserAsync();

        var response = await this._afterSalesCommentService.QueryPagingAsync(dto);

        return response;
    }

    [HttpPost("pending-count")]
    public async Task<ApiResponse<int>> AftersalePendingCountAsync()
    {
        var loginUser = await this.StoreAuthService.GetRequiredStoreUserAsync();

        var count = await this._aftersaleService.QueryPendingCountAsync(new QueryAftersalePendingCountInput()
            { UserId = loginUser.Id });

        return new ApiResponse<int>(count);
    }

    [HttpPost("cancel")]
    public async Task<ApiResponse<object>> CancelAsync([FromBody] CancelAftersaleInput dto)
    {
        var loginUser = await this.StoreAuthService.GetRequiredStoreUserAsync();

        var aftersales = await this._aftersaleService.QueryByIdAsync(dto.Id);
        if (aftersales == null || aftersales.UserId != loginUser.Id)
            throw new EntityNotFoundException();

        await this._aftersaleService.CancelAsync(dto);

        return new ApiResponse<object>();
    }

    [HttpPost("create")]
    public async Task<ApiResponse<AfterSalesDto>> CreateAftersalesAsync([FromBody] AfterSalesDto dto)
    {
        var loginUser = await this.StoreAuthService.GetRequiredStoreUserAsync();

        dto.UserId = loginUser.Id;

        var order = await this._orderService.QueryByIdAsync(dto.OrderId);

        if (order == null || order.UserId != loginUser.Id)
            throw new EntityNotFoundException();

        var response = await this._aftersaleService.InsertAsync(dto);

        return response;
    }

    [NonAction]
    private async Task AttachDataAsync(AfterSalesDto[] data)
    {
        if (!data.Any())
            return;

        await this._aftersaleService.AttachDataAsync(data, new AttachDataInput()
        {
            Order = true,
            Items = true
        });

        var items = data.SelectMany(x => x.Items).ToArray();

        if (!items.Any())
            return;

        await this._aftersaleService.AttachAfterSalesItemsDataAsync(items,
            new AttachAftersalesItemsDataInput() { OrderItems = true });

        var orderItems = items.Select(x => x.OrderItem).WhereNotNull().ToArray();

        if (!orderItems.Any())
            return;

        await this._orderService.AttachOrderItemDataAsync(orderItems, new OrderItemAttachDataInput() { Goods = true });

        var goods = orderItems.Select(x => x.Goods).WhereNotNull().ToArray();

        if (!goods.Any())
            return;

        await this._goodsService.AttachDataAsync(goods, new AttachGoodsDataInput() { Images = true });
    }

    [HttpPost("by-id")]
    public async Task<ApiResponse<AfterSalesDto>> QueryByIdAsync([FromBody] IdDto dto)
    {
        var loginUser = await this.StoreAuthService.GetRequiredStoreUserAsync();

        var afterSalesDto = await this._aftersaleService.QueryByIdAsync(dto.Id);

        if (afterSalesDto == null || afterSalesDto.UserId != loginUser.Id)
            throw new EntityNotFoundException();

        await this.AttachDataAsync(new[] { afterSalesDto });

        return new ApiResponse<AfterSalesDto>(afterSalesDto);
    }
}