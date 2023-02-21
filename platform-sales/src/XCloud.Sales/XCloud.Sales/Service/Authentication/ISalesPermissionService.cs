using XCloud.Core.Dto;
using XCloud.Platform.Member.Application.Service.AdminPermission;
using XCloud.Sales.Application;
using XCloud.Sales.Clients.Platform;

namespace XCloud.Sales.Service.Authentication;

public interface ISalesPermissionService : ISalesAppService
{
    Task<ApiResponse<object>> AuthorizeAsync(StoreAdministrator storeAdministrator, PermissionRecord permission);
}

public class SalesPermissionService : SalesAppService, ISalesPermissionService
{
    private readonly PlatformInternalService _platformInternalService;

    public SalesPermissionService(PlatformInternalService platformInternalService)
    {
        _platformInternalService = platformInternalService;
        //
    }

    public async Task<ApiResponse<object>> AuthorizeAsync(
        StoreAdministrator storeAdministrator,
        PermissionRecord permission)
    {
        if (storeAdministrator == null)
            throw new ArgumentNullException(nameof(storeAdministrator));

        if (permission == null)
            throw new ArgumentNullException(nameof(permission));

        var response =
            await this._platformInternalService.QueryAdminPermissionsAsync(storeAdministrator.AdministratorId);

        if (response.Data == null)
        {
            return new ApiResponse<object>().SetError("admin not exist");
        }

        if (response.Data.IsSuperAdmin)
        {
            return new ApiResponse<object>().ResetError();
        }

        //check permission
        var permissionKeys = response.GetAllPermissions();
        
        if (!permissionKeys.Contains(permission.Name) && !permissionKeys.Contains("*"))
        {
            //return new ApiResponse<object>().SetError($"permission:${permission.Name} is required");
            //ignore for now
        }

        return new ApiResponse<object>();
    }
}