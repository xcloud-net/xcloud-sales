using Volo.Abp.Application.Dtos;
using XCloud.Core.Dto;
using XCloud.Platform.Core.Domain.Logging;

namespace XCloud.Platform.Application.Common.Service.Logging;

public class ActivityLogDto : ActivityLog, IEntityDto<string>
{
    //
}

public class QueryLogPagingInput : PagedRequest
{
    //
}