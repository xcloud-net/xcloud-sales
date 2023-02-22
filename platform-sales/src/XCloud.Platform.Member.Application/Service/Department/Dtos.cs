using Volo.Abp.Application.Dtos;
using XCloud.Platform.Core.Domain.Department;

namespace XCloud.Platform.Member.Application.Service.Department;

public class SysDepartmentDto : SysDepartment, IEntityDto<string>
{
    //
}

public class JoinDepartmentDto : IEntityDto
{
    public string AdminId { get; set; }
    public string[] DepartmentIds { get; set; }
}

public class QueryAdminDepartmentsDto : IEntityDto
{
    public string AdminId { get; set; }
}