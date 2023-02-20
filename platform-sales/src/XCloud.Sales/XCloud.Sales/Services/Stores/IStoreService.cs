using XCloud.Database.EntityFrameworkCore.Extensions;
using XCloud.Sales.Data;
using XCloud.Sales.Data.Domain.Stores;

namespace XCloud.Sales.Services.Stores;

public interface IStoreService : ISalesPagingStringAppService<Store, StoreDto, QueryStorePagingInput>
{
    Task UpdateStatusAsync(UpdateStoreStatusInput dto);
}

public class StoreService : SalesPagingStringAppService<Store, StoreDto, QueryStorePagingInput>, IStoreService
{
    private readonly ISalesRepository<Store> _storeRepository;

    public StoreService(ISalesRepository<Store> storeRepository) : base(storeRepository)
    {
        this._storeRepository = storeRepository;
    }

    protected override async Task ModifyFieldsForUpdateAsync(Store store, StoreDto dto)
    {
        await Task.CompletedTask;
        
        store.StoreName = dto.StoreName;
        store.StoreUrl = dto.StoreUrl;
        store.StoreLogo = dto.StoreLogo;
        store.CopyrightInfo = dto.CopyrightInfo;
        store.ICPRecord = dto.ICPRecord;
        store.StoreServiceTime = dto.StoreServiceTime;
        store.ServiceTelePhone = dto.ServiceTelePhone;
    }

    protected override async Task InitBeforeInsertAsync(Store store)
    {
        await Task.CompletedTask;
        
        store.Id = this.GuidGenerator.CreateGuidString();
        store.CreationTime = this.Clock.Now;
    }

    protected override async Task CheckBeforeInsertAsync(StoreDto dto)
    {
        await Task.CompletedTask;

        if (string.IsNullOrWhiteSpace(dto.StoreName))
            throw new UserFriendlyException("store name is required");
    }

    protected override async Task CheckBeforeUpdateAsync(StoreDto dto)
    {
        await Task.CompletedTask;

        if (string.IsNullOrWhiteSpace(dto.StoreName))
            throw new UserFriendlyException("store name is required");
    }

    protected override async Task<IQueryable<Store>> GetFilteredQueryableAsync(IQueryable<Store> query, QueryStorePagingInput dto)
    {
        await Task.CompletedTask;
        return query;
    }

    public async Task UpdateStatusAsync(UpdateStoreStatusInput dto)
    {
        var db = await this._storeRepository.GetDbContextAsync();

        var store = await db.Set<Store>().IgnoreQueryFilters().FirstOrDefaultAsync(x => x.Id == dto.StoreId);
        if (store == null)
            throw new EntityNotFoundException(nameof(UpdateStatusAsync));

        if (dto.IsDeleted != null)
            store.IsDeleted = dto.IsDeleted.Value;

        if (dto.IsActive != null)
            store.IsActive = dto.IsActive.Value;

        if (dto.IsClosed != null)
            store.StoreClosed = dto.IsClosed.Value;

        await db.TrySaveChangesAsync();
    }
}