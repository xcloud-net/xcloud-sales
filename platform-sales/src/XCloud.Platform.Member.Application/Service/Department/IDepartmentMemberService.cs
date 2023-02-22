using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using XCloud.Core.Application;
using XCloud.Core.Dto;
using XCloud.Core.Helper;
using XCloud.Core.IdGenerator;
using XCloud.Platform.Core.Application;
using XCloud.Platform.Core.Domain.Admin;
using XCloud.Platform.Core.Domain.Department;
using XCloud.Platform.Data.Database;

namespace XCloud.Platform.Member.Application.Service.Department;

public interface IDepartmentMemberService : IXCloudApplicationService
{
    Task SetAdminDepartmentAsync(JoinDepartmentDto dto);

    Task<SysAdmin[]> QueryDepartmentAdminAsync(IdDto dto);

    Task<SysDepartment[]> QueryAdminDepartmentsAsync(QueryAdminDepartmentsDto dto);
}

public class DepartmentMemberService : PlatformApplicationService, IDepartmentMemberService
{
    private readonly IMemberRepository<SysDepartmentAssign> _departmentMemberRepository;

    public DepartmentMemberService(IMemberRepository<SysDepartmentAssign> departmentMemberRepository)
    {
        this._departmentMemberRepository = departmentMemberRepository;
    }

    public async Task SetAdminDepartmentAsync(JoinDepartmentDto dto)
    {
        if (string.IsNullOrWhiteSpace(dto.AdminId))
            throw new ArgumentNullException(nameof(SetAdminDepartmentAsync));

        await _departmentMemberRepository.DeleteNowAsync(x => x.AdminId == dto.AdminId);

        if (ValidateHelper.IsNotEmptyCollection(dto.DepartmentIds))
        {
            foreach (var m in dto.DepartmentIds)
            {
                var assignment = new SysDepartmentAssign()
                {
                    DepartmentId = m,
                    AdminId = dto.AdminId,
                    IsManager = false.ToBoolInt()
                };
                assignment.Id = this.GuidGenerator.CreateGuidString();
                assignment.CreationTime = this.Clock.Now;

                await _departmentMemberRepository.InsertNowAsync(assignment);
            }
        }
    }

    public async Task<SysAdmin[]> QueryDepartmentAdminAsync(IdDto dto)
    {
        if (dto == null || string.IsNullOrWhiteSpace(dto.Id))
            throw new ArgumentNullException(nameof(QueryDepartmentAdminAsync));

        var db = await _departmentMemberRepository.GetDbContextAsync();

        var query = from deptAssign in db.Set<SysDepartmentAssign>().AsNoTracking()
            join tenantMember in db.Set<SysAdmin>().AsNoTracking()
                on deptAssign.AdminId equals tenantMember.Id
            select new
            {
                deptAssign,
                tenantMember
            };

        query = query.Where(x => x.deptAssign.DepartmentId == dto.Id);

        var res = await query.Select(x => x.tenantMember).ToArrayAsync();

        return res;
    }

    public async Task<SysDepartment[]> QueryAdminDepartmentsAsync(QueryAdminDepartmentsDto dto)
    {
        var db = await _departmentMemberRepository.GetDbContextAsync();

        var query = from deptAssign in db.Set<SysDepartmentAssign>().AsNoTracking()
            join dept in db.Set<SysDepartment>().AsNoTracking()
                on new { deptId = deptAssign.DepartmentId } equals new { deptId = dept.Id }
            select new
            {
                deptAssign,
                dept
            };

        query = query.Where(x => x.deptAssign.AdminId == dto.AdminId);

        var departments = await query.Select(x => x.dept).ToArrayAsync();

        return departments.ToArray();
    }
}