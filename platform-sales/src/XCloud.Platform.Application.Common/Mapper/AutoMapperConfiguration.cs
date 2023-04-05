using AutoMapper;
using XCloud.Platform.Application.Common.Service.Address;
using XCloud.Platform.Application.Common.Service.AsyncJob;
using XCloud.Platform.Application.Common.Service.Logging;
using XCloud.Platform.Application.Common.Service.Notification;
using XCloud.Platform.Application.Common.Service.Settings;
using XCloud.Platform.Application.Common.Service.Storage;
using XCloud.Platform.Core.Domain.Address;
using XCloud.Platform.Core.Domain.AsyncJob;
using XCloud.Platform.Core.Domain.Logging;
using XCloud.Platform.Core.Domain.Notification;
using XCloud.Platform.Core.Domain.Settings;
using XCloud.Platform.Core.Domain.Storage;

namespace XCloud.Platform.Application.Common.Mapper;

public class AutoMapperConfiguration : Profile
{
    public AutoMapperConfiguration()
    {
        this.CreateMap<SysNotificationDto, SysNotification>();
        this.CreateMap<SysNotification, SysNotificationDto>();
        
        this.CreateMap<SysSettings, SysSettingsDto>().ReverseMap();
        this.CreateMap<StorageResourceMeta, StorageResourceMetaDto>().ReverseMap();
        this.CreateMap<JobRecord, JobRecordDto>().ReverseMap();
        this.CreateMap<ActivityLog, ActivityLogDto>().ReverseMap();
        this.CreateMap<UserAddress, UserAddressDto>().ReverseMap();
    }
}