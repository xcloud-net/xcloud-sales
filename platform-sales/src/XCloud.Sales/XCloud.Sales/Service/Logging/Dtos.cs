using Volo.Abp.Application.Dtos;
using XCloud.Core.Dto;
using XCloud.Platform.Application.Member.Service.Admin;
using XCloud.Sales.Data.Domain.Logging;
using XCloud.Sales.Service.Users;

namespace XCloud.Sales.Service.Logging;

public class ActivityLogSearchInput : PagedRequest
{
    public DateTime? StartTime { get; set; }

    public DateTime? EndTime { get; set; }

    public int? UserId { get; set; }

    public int? ActivityLogTypeId { get; set; }
}

public class ActivityLogDto : ActivityLog, IEntityDto
{
    public SysAdminDto Admin { get; set; }
    public StoreUserDto User { get; set; }
}