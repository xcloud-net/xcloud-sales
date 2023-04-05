using XCloud.Core.Application.Entity;
using XCloud.Platform.Core.Database;

namespace XCloud.Platform.Core.Domain.AsyncJob;

public class JobRecord : EntityBase, IPlatformEntity
{
    public string JobKey { get; set; }

    public string Desc { get; set; }

    public string ExceptionMessage { get; set; }

    public string ExtraData { get; set; }

    public DateTime? StartTime { get; set; }

    public DateTime? EndTime { get; set; }

    public int Status { get; set; }
}