using Microsoft.AspNetCore.Mvc;
using XCloud.Core.Dto;
using XCloud.Sales.Data.Domain.Users;
using XCloud.Sales.Service.Authentication;
using XCloud.Sales.Service.Users;

namespace XCloud.Sales.Mall.Api.Controllers.Admin;

[StoreAuditLog]
[Route("api/mall-admin/user-grade")]
public class UserGradeController : ShopBaseController
{
    private readonly IUserGradeService _userGradeService;

    public UserGradeController(IUserGradeService userGradeService)
    {
        this._userGradeService = userGradeService;
    }

    [HttpPost("set-user-grade")]
    public virtual async Task<ApiResponse<object>> SetUserGradeAsync([FromBody] UserGradeMappingDto dto)
    {
        var storeAdministrator = await this.StoreAuthService.GetRequiredStoreAdministratorAsync();

        await this.SalesPermissionService.CheckRequiredPermissionAsync(storeAdministrator,
            SalesPermissions.ManageUsers);

        await this._userGradeService.SetUserGradeAsync(dto);
        await this._userGradeService.UpdateGradeUserCountAsync(dto.GradeId);
        return new ApiResponse<object>();
    }

    [HttpPost("list")]
    public async Task<ApiResponse<UserGrade[]>> ListGradesAsync()
    {
        var storeAdministrator = await this.StoreAuthService.GetRequiredStoreAdministratorAsync();

        await this.SalesPermissionService.CheckRequiredPermissionAsync(storeAdministrator,
            SalesPermissions.ManageUsers);

        var data = await this._userGradeService.QueryAllGradesAsync();

        return new ApiResponse<UserGrade[]>(data);
    }

    [HttpPost("save")]
    public virtual async Task<ApiResponse<object>> SaveAsync([FromBody] UserGrade dto)
    {
        var storeAdministrator = await this.StoreAuthService.GetRequiredStoreAdministratorAsync();

        await this.SalesPermissionService.CheckRequiredPermissionAsync(storeAdministrator,
            SalesPermissions.ManageUsers);

        if (string.IsNullOrWhiteSpace(dto.Id))
        {
            var res = await this._userGradeService.AddUserGradeAsync(dto);
            res.ThrowIfErrorOccured();
        }
        else
        {
            await this._userGradeService.UpdateUserGradeAsync(dto);
        }
        return new ApiResponse<object>();
    }

    [HttpPost("update-status")]
    public virtual async Task<ApiResponse<object>> UpdateStatusAsync([FromBody] UpdateUserGradeStatusInput dto)
    {
        var storeAdministrator = await this.StoreAuthService.GetRequiredStoreAdministratorAsync();

        await this.SalesPermissionService.CheckRequiredPermissionAsync(storeAdministrator,
            SalesPermissions.ManageUsers);

        await this._userGradeService.UpdateUserGradeStatusAsync(dto);

        return new ApiResponse<object>();
    }
}