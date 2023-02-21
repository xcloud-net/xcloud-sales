using Microsoft.AspNetCore.Mvc;
using XCloud.Core.Dto;
using XCloud.Sales.Data.Domain.Orders;
using XCloud.Sales.Service.Authentication;
using XCloud.Sales.Service.Finance;
using XCloud.Sales.Service.Orders;
using XCloud.Sales.Service.Users;

namespace XCloud.Sales.Mall.Api.Controllers;

[Route("api/mall/balance")]
public class BalanceController : ShopBaseController
{
    private readonly IUserBalanceService _userBalanceService;
    private readonly IOrderBillService _orderBillService;
    private readonly IOrderService _orderService;
    private readonly IOrderProcessingService _orderProcessingService;
    private readonly IOrderPaymentProcessingService _orderPaymentProcessingService;

    /// <summary>
    /// 构造器
    /// </summary>
    public BalanceController(
        IOrderService orderService,
        IOrderProcessingService orderProcessingService,
        IOrderBillService orderBillService,
        IUserBalanceService userBalanceService,
        IOrderPaymentProcessingService orderPaymentProcessingService)
    {
        this._orderProcessingService = orderProcessingService;
        this._orderService = orderService;
        this._orderBillService = orderBillService;
        this._userBalanceService = userBalanceService;
        _orderPaymentProcessingService = orderPaymentProcessingService;
    }

    [HttpPost("create-order-payment")]
    public async Task<ApiResponse<object>> PayWithBalanceAsync([FromBody] IdDto orderId)
    {
        var loginUser = await this.StoreAuthService.GetRequiredStoreUserAsync();

        var order = await this._orderService.QueryByIdAsync(orderId.Id);

        if (order == null || order.UserId != loginUser.Id)
            throw new EntityNotFoundException();

        var bill = await this._orderPaymentProcessingService.CreateOrderPayBillAsync(new CreateOrderPayBillInput() { Id = order.Id });

        await this._userBalanceService.InsertBalanceHistoryAsync(new BalanceHistoryDto()
        {
            UserId = loginUser.Id,
            Balance = bill.Price,
            ActionType = (int)BalanceActionType.Use,
            Message = "used by order"
        });

        await this._orderBillService.MarkBillAsPayedAsync(new MarkBillAsPayedInput()
        {
            Id = bill.Id,
            PaymentMethod = (int)PaymentMethod.Balance
        });

        await this._orderPaymentProcessingService.TrySetAsPaidAfterBillPaidAsync(new IdDto(order.Id));

        return new ApiResponse<object>();
    }
}