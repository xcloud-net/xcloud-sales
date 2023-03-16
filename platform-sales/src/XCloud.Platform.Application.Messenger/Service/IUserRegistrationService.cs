using Microsoft.EntityFrameworkCore;
using Volo.Abp.Uow;
using XCloud.Application.Service;
using XCloud.Core.Cache;
using XCloud.Core.IdGenerator;
using XCloud.Platform.Core.Application;
using XCloud.Platform.Core.Domain.Messenger;
using XCloud.Platform.Data.Database;

namespace XCloud.Platform.Application.Messenger.Service;

public interface IUserRegistrationService : IXCloudApplicationService
{
    Task RegisterUserInfoAsync(SysUserOnlineStatusDto info);

    Task<string[]> GetUserServerInstancesAsync(string userId);

    Task RemoveRegisterInfoAsync(string userId, string device);
}

[UnitOfWork]
public class DatabaseUserRegistrationService : PlatformApplicationService, IUserRegistrationService
{
    private readonly IPlatformRepository<SysUserOnlineStatus> _repository;

    public DatabaseUserRegistrationService(IPlatformRepository<SysUserOnlineStatus> repository)
    {
        _repository = repository;
    }

    private async Task RegisterUserInfoImplAsync(SysUserOnlineStatusDto dto)
    {
        var entity = await this._repository.QueryOneAsync(x =>
            x.UserId == dto.UserId && x.DeviceId == dto.DeviceId && x.ServerInstanceId == dto.ServerInstanceId);

        if (entity == null)
        {
            entity = this.ObjectMapper.Map<SysUserOnlineStatusDto, SysUserOnlineStatus>(dto);
            entity.Id = this.GuidGenerator.CreateGuidString();
            entity.CreationTime = this.Clock.Now;

            await this._repository.InsertAsync(entity);
        }
        else
        {
            entity.PingTime = dto.PingTime;
            await this._repository.UpdateAsync(entity);
        }

        await this.CurrentUnitOfWork.SaveChangesAsync();
    }

    public async Task RegisterUserInfoAsync(SysUserOnlineStatusDto dto)
    {
        var key = $"{dto.UserId}.{dto.DeviceId}.{dto.ServerInstanceId}";

        await this.CacheProvider.GetOrSetAsync<string>(async () =>
        {
            await this.RegisterUserInfoImplAsync(dto);
            return string.Empty;
        }, new CacheOption<string>()
        {
            Expiration = TimeSpan.FromMinutes(1),
            Key = key
        });
    }

    public async Task<string[]> GetUserServerInstancesAsync(string userId)
    {
        if (string.IsNullOrWhiteSpace(userId))
            throw new ArgumentNullException(nameof(userId));

        var db = await this._repository.GetDbContextAsync();

        var query = db.Set<SysUserOnlineStatus>().AsNoTracking();

        query = query.Where(x => x.UserId == userId);

        var serverInstances = await query.Select(x => x.ServerInstanceId).Distinct().ToArrayAsync();

        return serverInstances;
    }

    public async Task RemoveRegisterInfoAsync(string userId, string device)
    {
        if (string.IsNullOrWhiteSpace(userId))
            throw new ArgumentNullException(nameof(userId));

        if (string.IsNullOrWhiteSpace(device))
            throw new ArgumentNullException(nameof(device));

        await this._repository.DeleteAsync(x => x.UserId == userId && x.DeviceId == device);
        await this.CurrentUnitOfWork.SaveChangesAsync();
    }
}