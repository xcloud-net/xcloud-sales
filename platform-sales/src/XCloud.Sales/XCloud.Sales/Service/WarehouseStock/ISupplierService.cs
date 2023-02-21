using XCloud.Core.Application;
using XCloud.Core.Dto;
using XCloud.Database.EntityFrameworkCore.Extensions;
using XCloud.Sales.Application;
using XCloud.Sales.Data;
using XCloud.Sales.Data.Domain.WarehouseStock;

namespace XCloud.Sales.Service.WarehouseStock;

public interface ISupplierService : ISalesAppService
{
    Task InsertAsync(SupplierDto dto);

    Task UpdateAsync(SupplierDto dto);
    
    Task UpdateStatusAsync(UpdateSupplierStatusInput dto);
    
    Task<PagedResponse<SupplierDto>> QueryPagingAsync(QuerySupplierPagingInput dto);

    Task<SupplierDto[]> QueryAllAsync();
}

public class SupplierService : SalesAppService, ISupplierService
{
    private readonly ISalesRepository<Supplier> _repository;

    public SupplierService(ISalesRepository<Supplier> repository)
    {
        _repository = repository;
    }
    
    public async Task UpdateStatusAsync(UpdateSupplierStatusInput dto)
    {
        if (dto == null)
            throw new ArgumentNullException(nameof(dto));

        if (string.IsNullOrWhiteSpace(dto.Id))
            throw new ArgumentNullException(nameof(dto.Id));
        
        var db = await this._repository.GetDbContextAsync();

        var entity =await db.Set<Supplier>().IgnoreQueryFilters().FirstOrDefaultAsync(x => x.Id == dto.Id);

        if (entity == null)
            throw new EntityNotFoundException(nameof(entity));

        if (dto.IsDeleted != null)
            entity.IsDeleted = dto.IsDeleted.Value;
        
        await db.TrySaveChangesAsync();
    }

    private string NormalizeName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentNullException(nameof(name));

        return name.Trim();
    }

    private async Task<bool> CheckNameIsExistAsync(string name, string exceptId = null)
    {
        var db = await this._repository.GetDbContextAsync();

        var query = db.Set<Supplier>().AsNoTracking();
        query = query.Where(x => x.Name == name);
        if (!string.IsNullOrWhiteSpace(exceptId))
            query = query.Where(x => x.Id != exceptId);

        var exist = await query.AnyAsync();

        return exist;
    }

    public async Task InsertAsync(SupplierDto dto)
    {
        if (dto == null)
            throw new ArgumentNullException(nameof(dto));

        if (string.IsNullOrWhiteSpace(dto.Name))
            throw new ArgumentNullException(nameof(dto.Name));

        dto.Name = this.NormalizeName(dto.Name);

        if (await this.CheckNameIsExistAsync(dto.Name))
            throw new UserFriendlyException("duplicate name");

        var db = await this._repository.GetDbContextAsync();

        var entity = this.ObjectMapper.Map<SupplierDto, Supplier>(dto);
        entity.Id = this.GuidGenerator.CreateGuidString();
        entity.CreationTime = this.Clock.Now;

        db.Set<Supplier>().Add(entity);

        await db.SaveChangesAsync();
    }

    public async Task UpdateAsync(SupplierDto dto)
    {
        if (dto == null)
            throw new ArgumentNullException(nameof(dto));

        if (string.IsNullOrWhiteSpace(dto.Id))
            throw new ArgumentNullException(nameof(dto.Id));

        if (string.IsNullOrWhiteSpace(dto.Name))
            throw new ArgumentNullException(nameof(dto.Name));

        dto.Name = this.NormalizeName(dto.Name);

        var db = await this._repository.GetDbContextAsync();

        var entity = await db.Set<Supplier>().FirstOrDefaultAsync(x => x.Id == dto.Id);

        if (entity == null)
            throw new EntityNotFoundException(nameof(entity));

        if (await this.CheckNameIsExistAsync(dto.Name, entity.Id))
            throw new UserFriendlyException("duplicate name");

        entity.Name = dto.Name;
        entity.Address = dto.Address;
        entity.Telephone = dto.Telephone;
        entity.ContactName = dto.ContactName;

        await db.TrySaveChangesAsync();
    }
    
    public async Task<SupplierDto[]> QueryAllAsync()
    {
        var db = await this._repository.GetDbContextAsync();

        var query = db.Set<Supplier>().AsNoTracking();
        
        var list = await query
            .OrderByDescending(x => x.CreationTime)
            .ToArrayAsync();

        var items = this.ObjectMapper.MapArray<Supplier, SupplierDto>(list);

        return items;
    }

    public async Task<PagedResponse<SupplierDto>> QueryPagingAsync(QuerySupplierPagingInput dto)
    {
        if (dto == null)
            throw new ArgumentNullException(nameof(dto));

        var db = await this._repository.GetDbContextAsync();

        var query = db.Set<Supplier>().AsNoTracking();

        var count = 0;
        if (!dto.SkipCalculateTotalCount)
            count = await query.CountAsync();

        var list = await query
            .OrderByDescending(x => x.CreationTime)
            .PageBy(dto.AsAbpPagedRequestDto())
            .ToArrayAsync();

        var items = this.ObjectMapper.MapArray<Supplier, SupplierDto>(list);

        return new PagedResponse<SupplierDto>(items, dto, count);
    }
}