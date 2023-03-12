using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using XCloud.Application.Service;
using XCloud.Core.Application;
using XCloud.Core.IdGenerator;
using XCloud.Platform.Core.Application;
using XCloud.Platform.Core.Domain.User;
using XCloud.Platform.Data.Database;

namespace XCloud.Platform.Member.Application.Service.User;

public interface IExternalConnectService : IXCloudApplicationService
{
    Task<SysExternalConnect> QueryByIdAsync(string connectionId);

    Task DeleteByIdAsync(string connectionId);

    Task SaveAsync(SysExternalConnect connection);

    Task<SysExternalConnect> FindByUserIdAsync(string platform, string appId, string userId);

    Task<SysExternalConnect> FindByOpenIdAsync(string platform, string appId, string openId);
}

public class ExternalConnectService : PlatformApplicationService, IExternalConnectService
{
    private readonly IMemberRepository<SysExternalConnect> _repository;

    public ExternalConnectService(IMemberRepository<SysExternalConnect> repository)
    {
        _repository = repository;
    }

    public async Task<SysExternalConnect> QueryByIdAsync(string connectionId)
    {
        if (string.IsNullOrWhiteSpace(connectionId))
            throw new ArgumentNullException(nameof(connectionId));

        var entity = await this._repository.QueryOneAsync(x => x.Id == connectionId);

        return entity;
    }

    public async Task DeleteByIdAsync(string connectionId)
    {
        if (string.IsNullOrWhiteSpace(connectionId))
            throw new ArgumentNullException(nameof(connectionId));

        await this._repository.DeleteAsync(x => x.Id == connectionId);
    }

    public async Task<SysExternalConnect> FindByUserIdAsync(string platform, string appId, string userId)
    {
        if (string.IsNullOrWhiteSpace(platform) || string.IsNullOrWhiteSpace(appId))
            throw new ArgumentNullException(nameof(FindByUserIdAsync));

        if (string.IsNullOrWhiteSpace(userId))
            throw new ArgumentNullException(nameof(userId));

        var db = await this._repository.GetDbContextAsync();

        var query = db.Set<SysExternalConnect>().AsNoTracking();

        query = query.Where(x => x.Platform == platform && x.AppId == appId && x.UserId == userId);

        var entity = await query.OrderBy(x => x.CreationTime).FirstOrDefaultAsync();

        return entity;
    }

    public async Task<SysExternalConnect> FindByOpenIdAsync(
        string platform, string appId, string openId)
    {
        if (string.IsNullOrWhiteSpace(platform) ||
            string.IsNullOrWhiteSpace(appId) ||
            string.IsNullOrWhiteSpace(openId))
            throw new ArgumentNullException(nameof(FindByOpenIdAsync));

        var db = await this._repository.GetDbContextAsync();

        var query = db.Set<SysExternalConnect>().AsNoTracking()
            .Where(x =>
                x.Platform == platform &&
                x.AppId == appId &&
                x.OpenId == openId);

        var res = await query.OrderBy(x => x.CreationTime).FirstOrDefaultAsync();

        return res;
    }

    public async Task SaveAsync(SysExternalConnect connection)
    {
        if (connection == null)
            throw new ArgumentNullException(nameof(connection));

        var db = await this._repository.GetDbContextAsync();

        var set = db.Set<SysExternalConnect>();

        var alreadyExist = await set.Where(x =>
            x.UserId == connection.UserId &&
            x.Platform == connection.Platform &&
            x.AppId == connection.AppId &&
            x.OpenId == connection.OpenId).AnyAsync();

        if (alreadyExist)
            return;

        connection.Id = this.GuidGenerator.CreateGuidString();
        connection.CreationTime = this.Clock.Now;

        set.Add(connection);

        await db.SaveChangesAsync();
    }
}