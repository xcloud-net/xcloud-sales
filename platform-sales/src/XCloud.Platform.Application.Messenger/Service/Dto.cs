using Volo.Abp.Application.Dtos;
using XCloud.Platform.Core.Domain.Messenger;

namespace XCloud.Platform.Application.Messenger.Service;

public class SysUserOnlineStatusDto : SysUserOnlineStatus, IEntityDto<string>
{
    //
}

public class ServerInstanceDto : SysServerInstance, IEntityDto<string>
{
    //
}