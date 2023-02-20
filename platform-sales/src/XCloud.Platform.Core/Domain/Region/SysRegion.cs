using XCloud.Core.Application;
using XCloud.Platform.Core.Database;

namespace XCloud.Platform.Core.Domain.Region;

public class SysRegion : TreeEntityBase, IPlatformEntity
{
    public string Name { get; set; }
    public string Code { get; set; }
    public string Data { get; set; }
    public string RegionType { get; set; }
    public int Sort { get; set; }
}