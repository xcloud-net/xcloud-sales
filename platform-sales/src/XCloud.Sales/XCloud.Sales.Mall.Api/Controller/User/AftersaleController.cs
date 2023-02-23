using Microsoft.AspNetCore.Mvc;
using XCloud.Core.Dto;
using XCloud.Sales.Service.AfterSale;
using XCloud.Sales.Service.Authentication;
using XCloud.Sales.Service.Catalog;
using XCloud.Sales.Service.Orders;

namespace XCloud.Sales.Mall.Api.Controller.User;

[Route("api/mall/aftersale")]
public class AfterSaleController : ShopBaseController
{
    private readonly IOrderService _orderService;
    private readonly IAfterSaleService _aftersaleService;
    private readonly IAfterSaleCommentService _afterSaleCommentService;
    private readonly IGoodsService _goodsService;

    public AfterSaleController(IOrderService orderService,
        IAfterSaleService aftersaleService,
        IGoodsService goodsService,
        IAfterSaleCommentService afterSaleCommentService)
    {
        this._aftersaleService = aftersaleService;
        this._orderService = orderService;
        this._goodsService = goodsService;
        _afterSaleCommentService = afterSaleCommentService;
    }

    [HttpPost("add-comment")]
    public async Task<ApiResponse<object>> InsertCommentAsync([FromBody] AfterSalesCommentDto dto)
    {
        var loginUser = await this.StoreAuthService.GetRequiredStoreUserAsync();

        var aftersale = await this._aftersaleService.QueryByIdAsync(dto.AfterSaleId);
        if (aftersale == null || aftersale.UserId != loginUser.Id)
            throw new EntityNotFoundException(nameof(aftersale));
        
        dto.IsAdmin = false;

        await this._afterSaleCommentService.InsertAsync(dto);

        return new ApiResponse<object>();
    }

    [HttpPost("comment-paging")]
    public async Task<PagedResponse<AfterSalesCommentDto>> QueryCommentPagingAsync(
        [FromBody] QueryAfterSalesCommentPagingInput dto)
    {
        var loginUser = await this.StoreAuthService.GetRequiredStoreUserAsync();

        var aftersale = await this._aftersaleService.QueryByIdAsync(dto.AfterSalesId);
        if (aftersale == null || aftersale.UserId != loginUser.Id)
            throw new EntityNotFoundException(nameof(aftersale));
        
        dto.SkipCalculateTotalCount = true;
        dto.PageSize = 10;

        var response = await this._afterSaleCommentService.QueryPagingAsync(dto);

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

        var order = await this._orderService.QueryByIdAsync(dto.OrderId);

        if (order == null || order.UserId != loginUser.Id)
            throw new EntityNotFoundException();

        dto.UserId = loginUser.Id;
        
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