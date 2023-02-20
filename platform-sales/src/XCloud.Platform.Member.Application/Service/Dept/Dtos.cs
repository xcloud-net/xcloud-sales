using Volo.Abp.Application.Dtos;

namespace XCloud.Platform.Member.Application.Service.Dept;

public class JoinDepartmentDto: IEntityDto
{
    public string AdminId { get; set; }
    public string[] DepartmentIds { get; set; }
}

public class QueryAdminDepartmentsDto : IEntityDto
{
    public string AdminId { get; set; }
}