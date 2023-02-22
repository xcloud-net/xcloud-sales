using Volo.Abp;
using Volo.Abp.Auditing;
using XCloud.Core.Application;
using XCloud.Platform.Core.Database;

namespace XCloud.Platform.Core.Domain.Department;

public class SysDepartment : TreeEntityBase, IMemberEntity, IHasCreationTime, ISoftDelete, 
    IHasDeletionTime,
    IHasModificationTime
{
    public string Name { get; set; }
    public string Description { get; set; }
    public string LogoUrl { get; set; }

    public bool IsDeleted { get; set; }

    public DateTime? DeletionTime { get; set; }

    public DateTime? LastModificationTime { get; set; }
}