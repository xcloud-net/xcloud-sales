using AutoMapper;
using XCloud.Platform.Application.Messenger.Message;
using XCloud.Platform.Application.Messenger.Service;
using XCloud.Platform.Core.Domain.Messenger;

namespace XCloud.Platform.Application.Messenger.Mapper;

public class AutoMapperConfiguration : Profile
{
    public AutoMapperConfiguration()
    {
        this.CreateMap<SysMessage, MessageDto>().ReverseMap();
        this.CreateMap<SysServerInstance, ServerInstanceDto>().ReverseMap();
        this.CreateMap<SysUserOnlineStatus, SysUserOnlineStatusDto>().ReverseMap();
    }
}