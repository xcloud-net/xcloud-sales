using System.IO;
using Volo.Abp.DependencyInjection;
using System.Threading.Tasks;
using XCloud.Platform.Application.Member.Service.User;
using XCloud.Platform.Core.Application;
using XCloud.Platform.Shared.Constants;
using XCloud.Platform.Shared.Helper;
using XCloud.Redis;

namespace XCloud.Platform.Application.Member.Job;

[UnitOfWork]
[ExposeServices(typeof(AutoResetUserPasswordJob))]
public class AutoResetUserPasswordJob : PlatformApplicationService, ITransientDependency
{
    private readonly IUserAccountService _userAccountService;
    private readonly MemberHelper _memberHelper;

    public AutoResetUserPasswordJob(IUserAccountService userAccountService, MemberHelper memberHelper)
    {
        _userAccountService = userAccountService;
        _memberHelper = memberHelper;
    }

    private async Task<string> ReadAndPruneUserPasswordFromDiskAsync()
    {
        var pwdPath = this.Configuration["app:config:passwordPath"];
        
        if (string.IsNullOrWhiteSpace(pwdPath))
            return null;
        
        if (!File.Exists(pwdPath))
            return null;

        var password = await File.ReadAllTextAsync(pwdPath);

        await File.WriteAllTextAsync(pwdPath, string.Empty);
        
        return password?.Trim().Trim('\n').Trim('\t').RemoveWhitespace();
    }

    private async Task TryUpdateUserPasswordAsync(string password)
    {
        var defaultUserName = IdentityConsts.Account.DefaultUserName;

        var user = await this._userAccountService.GetUserByIdentityNameAsync(defaultUserName);
        if (user == null)
            return;

        if (user.PassWord != this._memberHelper.EncryptPassword(password))
        {
            await this._userAccountService.SetPasswordAsync(user.Id, password);
        }
    }

    public virtual async Task ResetDefaultUserPasswordAsync()
    {
        using var dlock = await this.RedLockClient.RedLockFactory.CreateLockAsync(
            resource: $"{nameof(ExternalAccessTokenJob)}.{nameof(ResetDefaultUserPasswordAsync)}",
            TimeSpan.FromSeconds(5));
        
        if (dlock.IsAcquired)
        {
            var password = await this.ReadAndPruneUserPasswordFromDiskAsync();
            if (!string.IsNullOrWhiteSpace(password))
            {
                await this.TryUpdateUserPasswordAsync(password);
            }
        }
        else
        {
            throw new FailToGetRedLockException(nameof(ResetDefaultUserPasswordAsync));
        }
    }
}