using XCloud.Application.Extension;
using XCloud.Core.Dto;
using XCloud.Database.EntityFrameworkCore.Extensions;
using XCloud.Sales.Application;
using XCloud.Sales.Data;
using XCloud.Sales.Data.Domain.Stores;

namespace XCloud.Sales.Service.Stores;

public interface IStoreManagerService : ISalesAppService
{
    Task<StoreManager> QueryByIdAsync(string id);
    Task UpdateStatusAsync(UpdateStoreManagerStatusInput dto);
    Task<ApiResponse<StoreManager>> InsertAsync(StoreManager dto);
    Task<StoreManager[]> QueryByStoreIdAsync(string storeId);
    
    Task<StoreManager> QueryByGlobalUserIdAsync(string storeId, string globalUserId);

    Task<StoreManager> QueryByGlobalUserIdAsync(string storeId, string globalUserId,
        CachePolicy cachePolicyOption);
}

public class StoreManagerService : SalesAppService, IStoreManagerService
{
    private readonly ISalesRepository<StoreManager> storeManagerRepsotiroy;

    public StoreManagerService(ISalesRepository<StoreManager> storeManagerRepsotiroy)
    {
        this.storeManagerRepsotiroy = storeManagerRepsotiroy;
    }

    public async Task<StoreManager> QueryByGlobalUserIdAsync(string storeId, string globalUserId)
    {
        var db = await this.storeManagerRepsotiroy.GetDbContextAsync();

        var joinedQuery = from manager in db.Set<StoreManager>().AsNoTracking()
            join store in db.Set<Store>().AsNoTracking()
                on manager.StoreId equals store.Id
            select new
            {
                manager,
                store
            };

        joinedQuery = joinedQuery.Where(x => x.store.Id == storeId);
        joinedQuery = joinedQuery.Where(x => x.store.IsActive && x.manager.IsActive);
        joinedQuery = joinedQuery.Where(x => x.manager.GlobalUserId == globalUserId);

        var storeManager = await joinedQuery.Select(x => x.manager)
            .OrderBy(x => x.CreationTime).FirstOrDefaultAsync();

        return storeManager;
    }

    public async Task<StoreManager> QueryByGlobalUserIdAsync(string storeId, string globalUserId,
        CachePolicy cachePolicyOption)
    {
        var key = $"sales.store-manager.by.store-globaluser:{storeId},{globalUserId}";

        var storeManager = await this.CacheProvider.ExecuteWithPolicyAsync(
            () => this.QueryByGlobalUserIdAsync(storeId, globalUserId),
            new CacheOption<StoreManager>(key, TimeSpan.FromMinutes(1)) { CacheCondition = x => x != null },
            cachePolicyOption);

        return storeManager;
    }

    public async Task<StoreManager> QueryByIdAsync(string id)
    {
        var query = await this.storeManagerRepsotiroy.GetQueryableAsync();
        query = query.AsNoTracking();

        var manager = await query.FirstOrDefaultAsync(x => x.Id == id);
        return manager;
    }

    public async Task<ApiResponse<StoreManager>> InsertAsync(StoreManager dto)
    {
        var db = await this.storeManagerRepsotiroy.GetDbContextAsync();

        dto.Id = this.GuidGenerator.CreateGuidString();
        dto.CreationTime = this.Clock.Now;

        db.Set<StoreManager>().Add(dto);

        await db.SaveChangesAsync();
        return new ApiResponse<StoreManager>(dto);
    }

    public async Task UpdateStatusAsync(UpdateStoreManagerStatusInput dto)
    {
        var db = await this.storeManagerRepsotiroy.GetDbContextAsync();

        var manager = await db.Set<StoreManager>().FirstOrDefaultAsync(x => x.Id == dto.ManagerId);
        if (manager == null)
            throw new EntityNotFoundException(nameof(UpdateStatusAsync));

        if (dto.IsActive != null)
            manager.IsActive = dto.IsActive.Value;

        if (dto.IsDeleted != null)
            manager.IsDeleted = dto.IsDeleted.Value;

        await db.TrySaveChangesAsync();
    }

    public async Task<StoreManager[]> QueryByStoreIdAsync(string storeId)
    {
        var db = await this.storeManagerRepsotiroy.GetDbContextAsync();

        var data = await db.Set<StoreManager>().AsNoTracking()
            .Where(x => x.StoreId == storeId)
            .OrderByDescending(x => x.CreationTime)
            .TakeUpTo5000()
            .ToArrayAsync();

        return data;
    }
}