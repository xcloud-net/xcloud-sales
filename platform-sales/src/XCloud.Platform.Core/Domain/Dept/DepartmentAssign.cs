using XCloud.Core.Application;
using XCloud.Platform.Core.Database;

namespace XCloud.Platform.Core.Domain.Dept;

public class DepartmentAssign : EntityBase, IMemberEntity
{
    public string AdminId { get; set; }
    public string DepartmentId { get; set; }
    public int IsManager { get; set; }
}