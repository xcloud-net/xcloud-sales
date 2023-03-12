using Volo.Abp.Auditing;
using XCloud.Core.Application;
using XCloud.Core.Application.Entity;
using XCloud.Platform.Core.Database;

namespace XCloud.Platform.Core.Domain.IdGenerator;

public class SysSequence : EntityBase, IHasModificationTime, IPlatformEntity
{
    public string Category { get; set; }
    public string Description { get; set; }
    public int NextId { get; set; }
        
    public DateTime? LastModificationTime { get; set; }
}