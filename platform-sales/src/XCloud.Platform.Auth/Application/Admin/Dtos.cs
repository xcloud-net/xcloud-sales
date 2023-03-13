using Microsoft.AspNetCore.Authorization;
using Volo.Abp.Application.Dtos;
using XCloud.Platform.Application.Member.Service.Admin;

namespace XCloud.Platform.Auth.Application.Admin;

public class AdminPermissionRequirement : IAuthorizationRequirement, IEntityDto
{
    public SysAdminDto AdminDto { get; set; }

    public string[] Permissions { get; set; }

    public string[] Roles { get; set; }
}