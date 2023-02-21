using XCloud.Core.Application;
using XCloud.Core.Dto;
using XCloud.Database.EntityFrameworkCore.Extensions;
using XCloud.Sales.Application;
using XCloud.Sales.Data;
using XCloud.Sales.Data.Domain.WarehouseStock;

namespace XCloud.Sales.Service.WarehouseStock;

public interface IWarehouseService : ISalesAppService
{
    Task UpdateStatusAsync(UpdateWarehouseStatusInput dto);
    
    Task InsertAsync(WarehouseDto dto);

    Task UpdateAsync(WarehouseDto dto);

    Task<PagedResponse<WarehouseDto>> QueryPagingAsync(QueryWarehousePagingInput dto);

    Task<WarehouseDto[]> QueryAllAsync();
}

public class WarehouseService : SalesAppService, IWarehouseService
{
    private readonly ISalesRepository<Warehouse> _repository;

    public WarehouseService(ISalesRepository<Warehouse> repository)
    {
        _repository = repository;
    }

    public async Task UpdateStatusAsync(UpdateWarehouseStatusInput dto)
    {
        if (dto == null)
            throw new ArgumentNullException(nameof(dto));

        if (string.IsNullOrWhiteSpace(dto.Id))
            throw new ArgumentNullException(nameof(dto.Id));
        
        var db = await this._repository.GetDbContextAsync();

        var entity =await db.Set<Warehouse>().IgnoreQueryFilters().FirstOrDefaultAsync(x => x.Id == dto.Id);

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

        var query = db.Set<Warehouse>().AsNoTracking();
        query = query.Where(x => x.Name == name);
        if (!string.IsNullOrWhiteSpace(exceptId))
            query = query.Where(x => x.Id != exceptId);

        var exist = await query.AnyAsync();

        return exist;
    }

    public async Task InsertAsync(WarehouseDto dto)
    {
        if (dto == null)
            throw new ArgumentNullException(nameof(dto));

        if (string.IsNullOrWhiteSpace(dto.Name))
            throw new ArgumentNullException(nameof(dto.Name));

        dto.Name = this.NormalizeName(dto.Name);

        if (await this.CheckNameIsExistAsync(dto.Name))
            throw new UserFriendlyException("duplicate name");

        var db = await this._repository.GetDbContextAsync();

        var entity = this.ObjectMapper.Map<WarehouseDto, Warehouse>(dto);
        entity.Id = this.GuidGenerator.CreateGuidString();
        entity.CreationTime = this.Clock.Now;

        db.Set<Warehouse>().Add(entity);

        await db.SaveChangesAsync();
    }

    public async Task UpdateAsync(WarehouseDto dto)
    {
        if (dto == null)
            throw new ArgumentNullException(nameof(dto));

        if (string.IsNullOrWhiteSpace(dto.Id))
            throw new ArgumentNullException(nameof(dto.Id));

        if (string.IsNullOrWhiteSpace(dto.Name))
            throw new ArgumentNullException(nameof(dto.Name));

        dto.Name = this.NormalizeName(dto.Name);

        var db = await this._repository.GetDbContextAsync();

        var entity = await db.Set<Warehouse>().FirstOrDefaultAsync(x => x.Id == dto.Id);

        if (entity == null)
            throw new EntityNotFoundException(nameof(entity));

        if (await this.CheckNameIsExistAsync(dto.Name, entity.Id))
            throw new UserFriendlyException("duplicate name");

        entity.Name = dto.Name;
        entity.Address = dto.Address;
        entity.Lng = dto.Lng;
        entity.Lat = dto.Lat;

        await db.TrySaveChangesAsync();
    }

    public async Task<WarehouseDto[]> QueryAllAsync()
    {
        var db = await this._repository.GetDbContextAsync();

        var query = db.Set<Warehouse>().AsNoTracking();
        
        var list = await query
            .OrderByDescending(x => x.CreationTime)
            .ToArrayAsync();

        var items = this.ObjectMapper.MapArray<Warehouse, WarehouseDto>(list);

        return items;
    }

    public async Task<PagedResponse<WarehouseDto>> QueryPagingAsync(QueryWarehousePagingInput dto)
    {
        if (dto == null)
            throw new ArgumentNullException(nameof(dto));

        var db = await this._repository.GetDbContextAsync();

        var query = db.Set<Warehouse>().AsNoTracking();

        var count = 0;
        if (!dto.SkipCalculateTotalCount)
            count = await query.CountAsync();

        var list = await query
            .OrderByDescending(x => x.CreationTime)
            .PageBy(dto.AsAbpPagedRequestDto())
            .ToArrayAsync();

        var items = this.ObjectMapper.MapArray<Warehouse, WarehouseDto>(list);

        return new PagedResponse<WarehouseDto>(items, dto, count);
    }
}