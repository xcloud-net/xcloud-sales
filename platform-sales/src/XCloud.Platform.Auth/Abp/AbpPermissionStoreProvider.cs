using System;
using System.Threading.Tasks;
using Volo.Abp.Authorization.Permissions;

namespace XCloud.Platform.Auth.Abp;

public class AbpPermissionStoreProvider : IPermissionStore
{
    public Task<bool> IsGrantedAsync(string name, string providerName, string providerKey)
    {
        throw new NotImplementedException();
    }

    public Task<MultiplePermissionGrantResult> IsGrantedAsync(string[] names, string providerName, string providerKey)
    {
        throw new NotImplementedException();
    }
}