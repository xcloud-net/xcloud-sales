using Volo.Abp.Application.Dtos;
using XCloud.Core.Dto;
using XCloud.Platform.Core.Domain.AsyncJob;

namespace XCloud.Platform.Common.Application.Service.AsyncJob;

public class JobRecordDto : JobRecord, IEntityDto<string>
{
    //
}

public class QueryJobPagingInput : PagedRequest
{
    //
}