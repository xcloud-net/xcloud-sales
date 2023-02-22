using AutoMapper;
using XCloud.Platform.Core.Domain.Admin;
using XCloud.Platform.Core.Domain.Department;
using XCloud.Platform.Core.Domain.Security;
using XCloud.Platform.Core.Domain.User;
using XCloud.Platform.Member.Application.Service.Admin;
using XCloud.Platform.Member.Application.Service.Department;
using XCloud.Platform.Member.Application.Service.Security;
using XCloud.Platform.Member.Application.Service.User;

namespace XCloud.Platform.Member.Application.Mapper;

public class AutoMapperConfiguration : Profile
{
    public AutoMapperConfiguration()
    {
        this.CreateMap<SysExternalConnect, SysUserExternalConnectDto>().ReverseMap();
        this.CreateMap<SysExternalAccessToken, SysUserExternalAccessTokenDto>().ReverseMap();

        this.CreateMap<SysAdmin, SysAdminDto>();
        this.CreateMap<SysAdminDto, SysAdmin>();

        this.CreateMap<SysUser, SysUserDto>();
        this.CreateMap<SysUserDto, SysUser>();

        this.CreateMap<SysRole, SysRoleDto>().ReverseMap();
        this.CreateMap<SysPermission, SysPermissionDto>().ReverseMap();
        this.CreateMap<SysDepartment, SysDepartmentDto>().ReverseMap();
    }
}