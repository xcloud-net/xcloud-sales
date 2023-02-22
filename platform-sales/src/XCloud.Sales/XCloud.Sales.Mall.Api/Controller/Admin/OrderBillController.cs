using Microsoft.AspNetCore.Mvc;
using XCloud.Core.Dto;
using XCloud.Sales.Service.Authentication;
using XCloud.Sales.Service.Finance;
using XCloud.Sales.Service.Orders;

namespace XCloud.Sales.Mall.Api.Controller.Admin;

[StoreAuditLog]
[Route("api/mall-admin/order-bill")]
public class OrderBillController : ShopBaseController
{
    private readonly IOrderBillService _orderBillService;
    private readonly IOrderService _orderService;

    public OrderBillController(IOrderBillService orderBillService, IOrderService orderService)
    {
        _orderBillService = orderBillService;
        _orderService = orderService;
    }

    [HttpPost("paging")]
    public async Task<PagedResponse<OrderBillDto>> QueryPagingAsync([FromBody] QueryOrderBillPagingInput dto)
    {
        var storeAdministrator = await this.StoreAuthService.GetRequiredStoreAdministratorAsync();

        await this.SalesPermissionService.CheckRequiredPermissionAsync(storeAdministrator,
            SalesPermissions.ManageOrders);
        
        var response = await this._orderBillService.QueryPagingAsync(dto);

        if (response.IsNotEmpty)
        {
            var orders = response.Items.Where(x => x.Order != null).Select(x => x.Order).ToArray();
            await this._orderService.AttachDataV2Async(orders, new OrderAttachDataOption()
            {
                User = true
            });
        }

        return response;
    }
}