using Volo.Abp.Application.Dtos;
using XCloud.Core.Dto;
using XCloud.Platform.Core.Domain.Security;
using XCloud.Platform.Member.Application.Service.Admin;

namespace XCloud.Platform.Member.Application.Service.Security;

public class GrantedPermissionResponse : ApiResponse<SysAdminDto>, IEntityDto
{
    public string[] RecalledPermissions { get; set; }

    public string[] Permissions { get; set; }
}

public class GetGrantedPermissionInput : IEntityDto
{
    public GetGrantedPermissionInput()
    {
        //
    }

    public GetGrantedPermissionInput(string adminId) : this()
    {
        this.AdminId = adminId;
    }

    public string AdminId { get; set; }
    public string AppKey { get; set; }
}

public class SysRoleDto : SysRole, IEntityDto<string>
{
    public string[] PermissionKeys { get; set; }
}

public class QueryRolePagingInput : PagedRequest
{
    //
}

public class SysPermissionDto : SysPermission, IEntityDto<string>
{
    //
}

public class AssignAdminRolesInput : IEntityDto<string>
{
    public string Id { get; set; }
    public string[] RoleIds { get; set; }
}

public class AssignPermissionToRoleInput : IEntityDto<string>
{
    public string Id { get; set; }
    public string[] PermissionKeys { get; set; }
}