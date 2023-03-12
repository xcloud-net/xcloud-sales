using XCloud.Core.Application;
using XCloud.Core.Application.Entity;
using XCloud.Platform.Core.Database;

namespace XCloud.Platform.Core.Domain.Settings;

public class SysSettings : EntityBase, IPlatformEntity
{
    public string Name { get; set; }
    
    public string Value { get; set; }
}