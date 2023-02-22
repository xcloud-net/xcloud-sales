using XCloud.Core.Application;
using XCloud.Platform.Core.Database;

namespace XCloud.Platform.Core.Domain.Department;

public class SysDepartmentAssign : EntityBase, IMemberEntity
{
    public string AdminId { get; set; }
    public string DepartmentId { get; set; }
    public int IsManager { get; set; }
}