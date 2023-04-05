using XCloud.Core.Application.Entity;
using XCloud.Platform.Core.Database;

namespace XCloud.Platform.Core.Domain.Logging;

public class ActivityLog : EntityBase, IPlatformEntity
{
    public string SubjectId { get; set; }
    public string SubjectType { get; set; }
    public string LogType { get; set; }
    public string AppId { get; set; }
    public string ActionName { get; set; }
    public string Log { get; set; }
    public string ExceptionDetail { get; set; }
    public string Data { get; set; }
    public DateTime? LogTime { get; set; }
}