using Microsoft.AspNetCore.Mvc;
using XCloud.Sales.Service.Finance;

namespace XCloud.Sales.Mall.Api.Controller.Admin;

[StoreAuditLog]
[Route("api/mall-admin/payment")]
public class PaymentController : ShopBaseController
{
    private readonly IOrderBillService _orderBillService;
    public PaymentController(IOrderBillService orderBillService)
    {
        this._orderBillService = orderBillService;
    }
}