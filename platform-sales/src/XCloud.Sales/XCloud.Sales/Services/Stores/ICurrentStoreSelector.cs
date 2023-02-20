using Microsoft.AspNetCore.Http;

namespace XCloud.Sales.Services.Stores;

public interface ICurrentStoreSelector
{
    Task<string> GetStoreIdOrEmptyAsync();
}

public static class CurrentStoreSelectorExtension
{
    public static async Task<string> GetRequiredStoreIdAsync(this ICurrentStoreSelector currentStoreSelector)
    {
        var storeId = await currentStoreSelector.GetStoreIdOrEmptyAsync();
        if (string.IsNullOrWhiteSpace(storeId))
        {
            throw new UserFriendlyException(nameof(storeId));
        }

        return storeId;
    }
}

public class DevCurrentStoreSelector : ICurrentStoreSelector
{
    public async Task<string> GetStoreIdOrEmptyAsync()
    {
        await Task.CompletedTask;
        return "store-id";
    }
}

public class CurrentStoreSelector : ICurrentStoreSelector, IScopedDependency
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public CurrentStoreSelector(IHttpContextAccessor httpContextAccessor)
    {
        this._httpContextAccessor = httpContextAccessor;
    }

    public async Task<string> GetStoreIdOrEmptyAsync()
    {
        await Task.CompletedTask;

        if (_httpContextAccessor.HttpContext == null)
            throw new NotSupportedException(nameof(_httpContextAccessor.HttpContext));
            
        var success = _httpContextAccessor.HttpContext.Request.Headers.TryGetValue("selected_store", out var storeId);
        if (success)
        {
            return storeId;
        }
        return null;
    }
}