using Volo.Abp.Application.Dtos;
using XCloud.Core.Dto;
using XCloud.Platform.Core.Domain.Notification;

namespace XCloud.Platform.Application.Common.Service.Notification;

public class SysNotificationDto : SysNotification, IEntityDto
{
    //
}

public class QueryNotificationInput : PagedRequest
{
    public string AppKey { get; set; }
    public string UserId { get; set; }
}

public class UpdateNotificationStatusInput : IEntityDto<string>
{
    public string Id { get; set; }
    public bool? Read { get; set; }
}