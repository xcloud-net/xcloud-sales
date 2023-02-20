using Microsoft.AspNetCore.Mvc;
using XCloud.Core.Dto;
using XCloud.Sales.Data.Domain.Orders;
using XCloud.Sales.Services.Finance;
using XCloud.Sales.Services.Orders;
using XCloud.Sales.Services.Users;

namespace XCloud.Sales.Mall.Api.Controllers;

[Route("api/mall/balance")]
public class BalanceController : ShopBaseController
{
    private readonly IUserBalanceService userBalanceService;
    private readonly IOrderBillService orderBillService;
    private readonly IOrderService orderService;
    private readonly IOrderProcessingService orderProcessingService;

    /// <summary>
    /// 构造器
    /// </summary>
    public BalanceController(
        IOrderService orderService,
        IOrderProcessingService orderProcessingService,
        IOrderBillService orderBillService,
        IUserBalanceService userBalanceService)
    {
        this.orderProcessingService = orderProcessingService;
        this.orderService = orderService;
        this.orderBillService = orderBillService;
        this.userBalanceService = userBalanceService;
    }

    [HttpPost("create-order-payment")]
    public async Task<ApiResponse<object>> PayWithBalanceAsync([FromBody] IdDto orderId)
    {
        var loginUser = await this.StoreAuthService.GetRequiredStoreUserAsync();

        var order = await this.orderService.QueryByIdAsync(orderId.Id);

        if (order == null || order.UserId != loginUser.Id)
            throw new EntityNotFoundException();

        var bill = await this.orderProcessingService.CreateOrderPayBillAsync(new CreateOrderPayBillInput() { Id = order.Id });

        await this.userBalanceService.InsertBalanceHistoryAsync(new BalanceHistoryDto()
        {
            UserId = loginUser.Id,
            Balance = bill.Price,
            ActionType = (int)BalanceActionType.Use,
            Message = "used by order"
        });

        await this.orderBillService.MarkBillAsPayedAsync(new MarkBillAsPayedInput()
        {
            Id = bill.Id,
            PaymentMethod = (int)PaymentMethod.Balance
        });

        await this.orderProcessingService.TrySetAsPaidAfterBillPaidAsync(new IdDto(order.Id));

        return new ApiResponse<object>();
    }
}