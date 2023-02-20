using Microsoft.AspNetCore.Components;
using XCloud.Sales.Services.Orders;

namespace XCloud.Sales.Mall.Api.Controllers;

[Route("api/mall/wechat-payment")]
public class WechatPaymentController : ShopBaseController
{
    private readonly IOrderService _orderService;
    public WechatPaymentController(IOrderService orderService)
    {
        this._orderService = orderService;
    }
}