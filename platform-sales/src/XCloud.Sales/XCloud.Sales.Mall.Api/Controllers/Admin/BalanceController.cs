using Microsoft.AspNetCore.Mvc;
using XCloud.Core.Dto;
using XCloud.Sales.Service.Authentication;
using XCloud.Sales.Service.Users;

namespace XCloud.Sales.Mall.Api.Controllers.Admin;

[StoreAuditLog]
[Route("api/mall-admin/balance")]
public class BalanceController : ShopBaseController
{
    private readonly IUserBalanceService _userBalanceService;

    public BalanceController(IUserBalanceService userBalanceService)
    {
        this._userBalanceService = userBalanceService;
    }

    [Obsolete]
    [HttpPost("charge-or-deduct")]
    public async Task<ApiResponse<object>> ChargeOrDeductAsync([FromBody] BalanceHistoryDto dto)
    {
        var storeAdministrator = await this.StoreAuthService.GetRequiredStoreAdministratorAsync();

        await this.SalesPermissionService.CheckRequiredPermissionAsync(storeAdministrator,
            SalesPermissions.ManageFinance);

        await this._userBalanceService.InsertBalanceHistoryAsync(dto);

        return new ApiResponse<object>();
    }
}