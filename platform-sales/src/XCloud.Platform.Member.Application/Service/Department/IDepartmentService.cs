using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Volo.Abp.Domain.Entities;
using XCloud.Application.Service;
using XCloud.Core.Application;
using XCloud.Core.Application.Entity;
using XCloud.Core.Dto;
using XCloud.Core.IdGenerator;
using XCloud.Database.EntityFrameworkCore.Extensions;
using XCloud.Platform.Core.Application;
using XCloud.Platform.Core.Domain.Department;
using XCloud.Platform.Data.Database;

namespace XCloud.Platform.Member.Application.Service.Department;

public interface IDepartmentService : IXCloudApplicationService
{
    Task<AntDesignTreeNode[]> QueryTreeAsync(IdDto dto);
    
    Task<SysDepartment> InsertAsync(SysDepartment entity);
    
    Task<SysDepartment> UpdateAsync(SysDepartment model);
    
    Task DeleteByIdAsync(IdDto dto);
    
    Task<SysDepartment> QueryByIdAsync(IdDto dto);
}

public class DepartmentService : PlatformApplicationService, IDepartmentService
{
    private readonly IMemberRepository<SysDepartment> _tenantDepartmentRepository;
    public DepartmentService(IMemberRepository<SysDepartment> tenantDepartmentRepository)
    {
        this._tenantDepartmentRepository = tenantDepartmentRepository;
    }

    public async Task<SysDepartment> InsertAsync(SysDepartment entity)
    {
        if (entity == null)
            throw new ArgumentNullException(nameof(entity));
        
        if (string.IsNullOrWhiteSpace(entity.Name))
        {
            throw new UserFriendlyException("部门名称为空");
        }

        entity.Id = this.GuidGenerator.CreateGuidString();
        entity.CreationTime = this.Clock.Now;
        var res = await _tenantDepartmentRepository.AddTreeNode(entity);
        res.ThrowIfErrorOccured();

        return entity;
    }

    public async Task<SysDepartment> UpdateAsync(SysDepartment model)
    {
        if (model == null)
            throw new ArgumentNullException(nameof(model));

        if (string.IsNullOrWhiteSpace(model.Id))
            throw new ArgumentNullException(nameof(model.Id));
        
        if (string.IsNullOrWhiteSpace(model.Name))
            throw new UserFriendlyException("部门名称为空");

        var entity = await _tenantDepartmentRepository.QueryOneAsync(x => x.Id == model.Id);

        if (entity == null)
            throw new EntityNotFoundException(nameof(UpdateAsync));

        entity.Name = model.Name;
        entity.Description = model.Description;
        entity.LogoUrl = model.LogoUrl;

        entity.LastModificationTime = this.Clock.Now;

        await _tenantDepartmentRepository.UpdateNowAsync(entity);

        return entity;
    }

    public async Task DeleteByIdAsync(IdDto dto)
    {
        if (string.IsNullOrWhiteSpace(dto?.Id))
            throw new ArgumentNullException(nameof(DeleteByIdAsync));

        var department = await _tenantDepartmentRepository.QueryOneAsync(x => x.Id == dto.Id);

        if (department == null)
            throw new EntityNotFoundException(nameof(DeleteByIdAsync));

        var res = await _tenantDepartmentRepository.DeleteSingleNodeWhenNoChildren(dto.Id);

        if (!res)
        {
            throw new UserFriendlyException("删除失败，请确保没有子节点");
        }
    }

    public async Task<AntDesignTreeNode[]> QueryTreeAsync(IdDto dto)
    {
        if (dto == null)
            throw new ArgumentNullException(nameof(dto));

        if (string.IsNullOrWhiteSpace(dto.Id))
            throw new ArgumentNullException(nameof(dto.Id));

        var db = await this._tenantDepartmentRepository.GetDbContextAsync();

        var query = db.Set<SysDepartment>().AsNoTracking();

        var data = await query.Take(1000).ToArrayAsync();

        var nodes = data.BuildAntTree(x => x.Name).ToArray();

        return nodes;
    }

    public async Task<SysDepartment> QueryByIdAsync(IdDto dto)
    {
        if (dto == null)
            throw new ArgumentNullException(nameof(dto));

        if (string.IsNullOrWhiteSpace(dto.Id))
            throw new ArgumentNullException(nameof(dto.Id));

        var data = await _tenantDepartmentRepository.QueryOneAsync(x => x.Id == dto.Id);

        return data;
    }
}