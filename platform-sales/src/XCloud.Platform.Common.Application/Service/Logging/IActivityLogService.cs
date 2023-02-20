using XCloud.Platform.Core.Application;
using XCloud.Platform.Core.Domain.Logging;
using XCloud.Platform.Data.Database;

namespace XCloud.Platform.Common.Application.Service.Logging;

public interface IActivityLogService : IPlatformPagingCrudService<ActivityLog, ActivityLogDto, QueryLogPagingInput>
{
    //
}

public class ActivityLogService : PlatformPagingCrudService<ActivityLog, ActivityLogDto, QueryLogPagingInput>,
    IActivityLogService
{
    public ActivityLogService(IPlatformRepository<ActivityLog> repository) : base(repository)
    {
        //
    }
}