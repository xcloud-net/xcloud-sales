using System;
using System.Security.Claims;
using System.Threading.Tasks;
using Volo.Abp.Authorization.Permissions;

namespace XCloud.Platform.Auth.Abp;

public class AbpPermissionCheckerProvider : IPermissionChecker
{
    public Task<bool> IsGrantedAsync(string name)
    {
        throw new NotImplementedException();
    }

    public Task<bool> IsGrantedAsync(ClaimsPrincipal claimsPrincipal, string name)
    {
        throw new NotImplementedException();
    }

    public Task<MultiplePermissionGrantResult> IsGrantedAsync(string[] names)
    {
        throw new NotImplementedException();
    }

    public Task<MultiplePermissionGrantResult> IsGrantedAsync(ClaimsPrincipal claimsPrincipal, string[] names)
    {
        throw new NotImplementedException();
    }
}