using Microsoft.AspNetCore.Components;
using XCloud.Sales.Service.Orders;

namespace XCloud.Sales.Mall.Api.Controller.User.Wechat.Mp;

[Route("api/mall/wechat-mp-payment")]
public class WechatPaymentController : ShopBaseController
{
    private readonly IOrderService _orderService;
    public WechatPaymentController(IOrderService orderService)
    {
        this._orderService = orderService;
    }
}