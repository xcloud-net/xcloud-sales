using AutoMapper;
using XCloud.Platform.Core.Domain.Admin;
using XCloud.Platform.Core.Domain.AdminPermission;
using XCloud.Platform.Core.Domain.User;
using XCloud.Platform.Member.Application.Service.Admin;
using XCloud.Platform.Member.Application.Service.AdminPermission;
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
    }
}