using Microsoft.AspNetCore.Mvc;
using XCloud.Core.Dto;
using XCloud.Sales.Data.Domain.Stores;
using XCloud.Sales.Service.Authentication;
using XCloud.Sales.Service.Stores;

namespace XCloud.Sales.Mall.Api.Controllers.Admin;

[StoreAuditLog]
[Route("api/mall-admin/store")]
public class AdminStoreController : ShopBaseController
{
    private readonly IStoreService _storeService;
    private readonly IStoreManagerService _storeManagerService;

    public AdminStoreController(IStoreService storeService,
        IStoreManagerService storeManagerService)
    {
        this._storeService = storeService;
        this._storeManagerService = storeManagerService;
    }

    [HttpPost("list")]
    public async Task<ApiResponse<StoreDto[]>> QueryStores()
    {
        var storeAdministrator = await this.StoreAuthService.GetRequiredStoreAdministratorAsync();

        await this.SalesPermissionService.CheckRequiredPermissionAsync(storeAdministrator,
            SalesPermissions.ManageStores);

        var response = await this._storeService.QueryPagingAsync(new QueryStorePagingInput()
        {
            Page = 1,
            PageSize = 1000,
            SkipCalculateTotalCount = true
        });

        if (response.IsEmpty)
        {
            return new ApiResponse<StoreDto[]>();
        }

        return new ApiResponse<StoreDto[]>(response.Items.ToArray());
    }

    [HttpPost("save")]
    public virtual async Task<ApiResponse<object>> CreateStore([FromBody] StoreDto dto)
    {
        var storeAdministrator = await this.StoreAuthService.GetRequiredStoreAdministratorAsync();

        await this.SalesPermissionService.CheckRequiredPermissionAsync(storeAdministrator,
            SalesPermissions.ManageStores);

        if (string.IsNullOrWhiteSpace(dto.Id))
        {
            await this._storeService.InsertAsync(dto);
        }
        else
        {
            await this._storeService.UpdateAsync(dto);
        }

        return new ApiResponse<object>();
    }

    [HttpPost("update-status")]
    public virtual async Task<ApiResponse<object>> UpdateStatus([FromBody] UpdateStoreStatusInput dto)
    {
        var storeAdministrator = await this.StoreAuthService.GetRequiredStoreAdministratorAsync();

        await this.SalesPermissionService.CheckRequiredPermissionAsync(storeAdministrator,
            SalesPermissions.ManageStores);

        var store = await this._storeService.QueryByIdAsync(dto.StoreId);

        if (store == null)
            throw new EntityNotFoundException(nameof(store));

        await this._storeService.UpdateStatusAsync(dto);

        return new ApiResponse<object>();
    }

    [HttpPost("delete")]
    public virtual async Task<ApiResponse<object>> DeleteStore([FromBody] IdDto dto)
    {
        var storeAdministrator = await this.StoreAuthService.GetRequiredStoreAdministratorAsync();

        await this.SalesPermissionService.CheckRequiredPermissionAsync(storeAdministrator,
            SalesPermissions.ManageStores);

        var store = await this._storeService.QueryByIdAsync(dto.Id);

        if (store == null)
            throw new EntityNotFoundException(nameof(store));

        await this._storeService.UpdateStatusAsync(new UpdateStoreStatusInput()
        {
            StoreId = store.Id,
            IsDeleted = true
        });

        return new ApiResponse<object>();
    }

    [HttpPost("manager-list")]
    public async Task<ApiResponse<StoreManager[]>> QueryStoreManager([FromBody] IdDto dto)
    {
        var storeAdministrator = await this.StoreAuthService.GetRequiredStoreAdministratorAsync();

        await this.SalesPermissionService.CheckRequiredPermissionAsync(storeAdministrator,
            SalesPermissions.ManageStores);

        var store = await this._storeService.QueryByIdAsync(dto.Id);

        if (store == null)
            throw new EntityNotFoundException(nameof(store));

        var managers = await this._storeManagerService.QueryByStoreIdAsync(dto.Id);

        return new ApiResponse<StoreManager[]>(managers);
    }

    [HttpPost("create-manager")]
    public virtual async Task<ApiResponse<StoreManager>> CreateStoreManager([FromBody] StoreManager dto)
    {
        var storeAdministrator = await this.StoreAuthService.GetRequiredStoreAdministratorAsync();

        await this.SalesPermissionService.CheckRequiredPermissionAsync(storeAdministrator,
            SalesPermissions.ManageStores);

        var store = await this._storeService.QueryByIdAsync(dto.StoreId);

        if (store == null)
            throw new EntityNotFoundException(nameof(store));

        var response = await this._storeManagerService.InsertAsync(dto);

        return response;
    }

    [HttpPost("delete-manager")]
    public virtual async Task<ApiResponse<object>> DeleteStoreManager([FromBody] IdDto dto)
    {
        var storeAdministrator = await this.StoreAuthService.GetRequiredStoreAdministratorAsync();

        await this.SalesPermissionService.CheckRequiredPermissionAsync(storeAdministrator,
            SalesPermissions.ManageStores);

        var manager = await this._storeManagerService.QueryByIdAsync(dto.Id);

        if (manager == null)
            throw new EntityNotFoundException(nameof(manager));

        var store = await this._storeService.QueryByIdAsync(manager.StoreId);

        if (store == null)
            throw new EntityNotFoundException(nameof(store));

        await this._storeManagerService.UpdateStatusAsync(new UpdateStoreManagerStatusInput()
        {
            ManagerId = manager.Id,
            IsDeleted = true,
            IsActive = false
        });

        return new ApiResponse<object>();
    }
}