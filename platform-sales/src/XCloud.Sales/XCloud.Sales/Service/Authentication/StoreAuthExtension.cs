using Volo.Abp.Authorization;
using XCloud.Core.Dto;
using XCloud.Platform.Shared.Exceptions;
using XCloud.Sales.Data.Domain.Stores;
using XCloud.Sales.Data.Domain.Users;

namespace XCloud.Sales.Service.Authentication;

public static class StoreAuthExtension
{
    public static async Task CheckRequiredPermissionAsync(this ISalesPermissionService salesPermissionService,
        StoreAdministrator storeAdministrator,
        PermissionRecord permission)
    {
        var response = await salesPermissionService.AuthorizeAsync(storeAdministrator, permission);
        if (!response.IsSuccess())
        {
            throw new NoPermissionException("no permission for this request");
        }
    }

    public static async Task<StoreManager> GetRequiredStoreManagerAsync(this IStoreAuthService storeAuthService,
        string storeId = null)
    {
        var res = await storeAuthService.GetStoreManagerAsync(storeId);

        if (res.IsSuccess())
        {
            var storeManager = res.Data;

            return storeManager;
        }

        if (res.SelectStoreRequired)
            throw new AbpAuthorizationException(res.Error.Message);

        if (res.NotTenantMember)
            throw new AbpAuthorizationException(res.Error.Message);

        if (res.NotStoreManager)
            throw new AbpAuthorizationException(res.Error.Message);

        throw new AbpAuthorizationException(res.Error.Message);
    }

    public static async Task<User> GetRequiredStoreUserAsync(this IStoreAuthService storeAuthService)
    {
        var res = await storeAuthService.GetStoreUserAsync();

        if (res.IsSuccess())
        {
            var storeUser = res.Data;
            return storeUser;
        }

        if (res.StoreUserIsNotValid)
            throw new AbpAuthorizationException(res.Error.Message);

        if (res.GlobalUserIsNotValid)
            throw new AbpAuthorizationException(res.Error.Message);

        throw new AbpAuthorizationException(res.Error.Message);
    }

    public static async Task<StoreAdministrator> GetRequiredStoreAdministratorAsync(
        this IStoreAuthService storeAuthService)
    {
        var res = await storeAuthService.GetStoreAdministratorAsync();
        if (res.IsSuccess())
            return res.Data;

        throw new AbpAuthorizationException(res.Error.Message);
    }

    public static async Task<User> GetStoreUserOrNullAsync(this IStoreAuthService authenticationService)
    {
        var storeUser = await authenticationService.GetStoreUserAsync();
        if (storeUser.IsSuccess())
            return storeUser.Data;

        return null;
    }
}