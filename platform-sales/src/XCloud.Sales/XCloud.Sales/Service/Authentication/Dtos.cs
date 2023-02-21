using Volo.Abp.Application.Dtos;
using XCloud.Core.Dto;
using XCloud.Platform.Member.Application.Service.User;
using XCloud.Sales.Data.Domain.Stores;
using XCloud.Sales.Data.Domain.Users;

namespace XCloud.Sales.Service.Authentication;

public class PermissionRecord : IEntityDto
{
    public string Name { get; set; }

    public string SystemName { get; set; }

    public string Group { get; set; }
}

public static class SalesPermissions
{
    public static readonly PermissionRecord ManageStock = new PermissionRecord
    {
        Name = "ManageStock",
        SystemName = "ManageStock",
        Group = "Standard"
    };

    public static readonly PermissionRecord ManageReport = new PermissionRecord
    {
        Name = "ManageReport",
        SystemName = "ManageReport",
        Group = "Standard"
    };
    
    public static readonly PermissionRecord ManagePromotion = new PermissionRecord
    {
        Name = "ManageCollection",
        SystemName = "ManageCollection",
        Group = "Standard"
    };
    
    public static readonly PermissionRecord ManageCollection = new PermissionRecord
    {
        Name = "ManageCollection",
        SystemName = "ManageCollection",
        Group = "Standard"
    };
    
    public static readonly PermissionRecord ManagePages = new PermissionRecord
    {
        Name = "ManagePages",
        SystemName = "ManagePages",
        Group = "Standard"
    };
    
    public static readonly PermissionRecord ManageFinance = new PermissionRecord
    {
        Name = "ManageFinance",
        SystemName = "ManageFinance",
        Group = "Standard"
    };

    public static readonly PermissionRecord ManageStores = new PermissionRecord
    {
        Name = "Manage Stores",
        SystemName = "ManageStore",
        Group = "Standard"
    };

    public static readonly PermissionRecord ManageBrands = new PermissionRecord
    {
        Name = "Admin area. Manage Brand",
        SystemName = "ManageBrand",
        Group = "Catalog"
    };

    public static readonly PermissionRecord ManageCatalog = new PermissionRecord
    {
        Name = "Admin area. Manage Catalog",
        SystemName = "ManageCatalog",
        Group = "Catalog"
    };

    public static readonly PermissionRecord ManageUsers = new PermissionRecord
    {
        Name = "Admin area. Manage Users",
        SystemName = "ManageUsers",
        Group = "Users"
    };

    public static readonly PermissionRecord ManageOrders = new PermissionRecord
    {
        Name = "Admin area. Manage Orders",
        SystemName = "ManageOrders",
        Group = "Orders"
    };

    public static readonly PermissionRecord ManageAfterSales = new PermissionRecord
    {
        Name = "Admin area. Manage Return Requests",
        SystemName = "ManageAfterSales",
        Group = "Orders"
    };

    public static readonly PermissionRecord ManageCoupons = new PermissionRecord
    {
        Name = "Admin area. Manage Coupon",
        SystemName = "ManageCoupons",
        Group = "Promo"
    };

    public static readonly PermissionRecord ManageSettings = new PermissionRecord
    {
        Name = "Admin area. Manage Settings",
        SystemName = "ManageSettings",
        Group = "Configuration"
    };

    public static readonly PermissionRecord ManageActivityLog = new PermissionRecord
    {
        Name = "Admin area. Manage Activity Log",
        SystemName = "ManageActivityLog",
        Group = "Configuration"
    };
}

public class AuthedGlobalUserResult : ApiResponse<SysUserDto>
{
    public bool TokenIsRequired { get; set; }
    public bool GlobalUserIsNotValid { get; set; }
}

public class AuthedStoreUserResult : ApiResponse<User>
{
    public bool GlobalUserIsNotValid { get; set; }
    public bool StoreUserIsNotValid { get; set; }

    public AuthedStoreUserResult()
    {
    }

    public AuthedStoreUserResult(User storeUser)
    {
        this.Data = storeUser;
    }

    public static implicit operator AuthedStoreUserResult(User storeUser) => new AuthedStoreUserResult(storeUser);
}

public class AuthedStoreManagerResult : ApiResponse<StoreManager>
{
    public bool SelectStoreRequired { get; set; }
    public bool NotTenantMember { get; set; }
    public bool NotStoreManager { get; set; }

    public AuthedStoreManagerResult()
    {
    }

    public AuthedStoreManagerResult(StoreManager storeManager)
    {
        this.Data = storeManager;
    }
}

public class StoreAdministrator : IEntityDto
{
    public StoreAdministrator()
    {
        //
    }

    public StoreAdministrator(string administratorId) : this()
    {
        this.AdministratorId = administratorId;
    }

    public string AdministratorId { get; set; }
}

public class AuthedStoreAdministratorResult : ApiResponse<StoreAdministrator>
{
    //
}