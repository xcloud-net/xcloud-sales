using XCloud.Core.Application;
using XCloud.Platform.Core.Database;
using XCloud.Platform.Shared;

namespace XCloud.Platform.Core.Domain.App;

public class SysAppScope : EntityBase, IPlatformEntity, IHasAppKey
{
    public string SubjectId { get; set; }
    public string SubjectType { get; set; }
    public string AppKey { get; set; }
}