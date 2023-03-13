using XCloud.Core.Application.Entity;
using XCloud.Platform.Core.Database;
using XCloud.Platform.Shared.Entity;

namespace XCloud.Platform.Core.Domain.App;

public class SysApp : EntityBase, IPlatformEntity, IHasAppKey
{
    public string Name { get; set; }
    public string AppKey { get; set; }
    public string Description { get; set; }
    public bool Enabled { get; set; }
}