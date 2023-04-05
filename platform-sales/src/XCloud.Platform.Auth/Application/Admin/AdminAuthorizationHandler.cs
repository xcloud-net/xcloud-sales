using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Logging;
using XCloud.Core.Application.Entity;
using XCloud.Core.Cache;
using XCloud.Core.Helper;
using XCloud.Platform.Application.Member.Service.Admin;
using XCloud.Platform.Application.Member.Service.Permission;
using XCloud.Platform.Application.Member.Service.Role;
using XCloud.Platform.Application.Member.Service.Security;

namespace XCloud.Platform.Auth.Application.Admin;

public class AdminAuthorizationHandler : AuthorizationHandler<AdminPermissionRequirement>
{
    private readonly ILogger _logger;
    private readonly IAdminSecurityService _securityService;
    private readonly IAdminRoleService _adminRoleService;

    public AdminAuthorizationHandler(
        ILogger<AdminAuthorizationHandler> logger,
        IAdminSecurityService securityService,
        IAdminRoleService adminRoleService)
    {
        this._logger = logger;
        this._securityService = securityService;
        _adminRoleService = adminRoleService;
    }

    protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context,
        AdminPermissionRequirement requirement)
    {
        if (context.Resource is SysAdminDto adminDto)
        {
            //
        }

        if (requirement.AdminDto == null)
            throw new ArgumentNullException(nameof(requirement.AdminDto));

        if (requirement.IsEmpty())
            throw new ArgumentNullException(nameof(requirement.Permissions));

        //as super admin
        if (requirement.AdminDto.IsSuperAdmin)
        {
            this._logger.LogInformation("as super admin");
            context.Succeed(requirement);
            return;
        }

        //check permissions
        if (ValidateHelper.IsNotEmptyCollection(requirement.Permissions))
        {
            var dto = new GetGrantedPermissionInput(requirement.AdminDto.Id);

            var policy = new CachePolicy() { Cache = true };

            var permissionDto = await this._securityService.GetGrantedPermissionsAsync(dto, policy);

            var allPermissions = permissionDto.GetAllPermissions();

            var requiredPermissions = requirement.Permissions.Except(allPermissions).ToArray();
            if (requiredPermissions.Any() && !allPermissions.Contains("*"))
            {
                this._logger.LogInformation($"required permissions:{string.Join(',', requiredPermissions)}");
                context.Fail();
                return;
            }
        }

        //check roles
        if (ValidateHelper.IsNotEmptyCollection(requirement.Roles))
        {
            var roles = await this._adminRoleService.QueryByAdminIdAsync(requirement.AdminDto.Id,
                new CachePolicy() { Cache = true });

            var roleIds = roles.Ids().ToArray();

            if (requirement.Roles.Except(roleIds).Any())
            {
                this._logger.LogInformation("roles required");
                context.Fail();
                return;
            }
        }

        this._logger.LogInformation("permission requirement meet");
        context.Succeed(requirement);
    }
}