using AutoMapper;
using XCloud.Platform.Application.Member.Service.Admin;
using XCloud.Platform.Application.Member.Service.Department;
using XCloud.Platform.Application.Member.Service.Security;
using XCloud.Platform.Application.Member.Service.User;
using XCloud.Platform.Core.Domain.Admin;
using XCloud.Platform.Core.Domain.Department;
using XCloud.Platform.Core.Domain.Security;
using XCloud.Platform.Core.Domain.User;

namespace XCloud.Platform.Application.Member.Mapper;

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