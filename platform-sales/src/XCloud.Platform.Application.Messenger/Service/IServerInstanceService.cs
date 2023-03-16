using Microsoft.EntityFrameworkCore;
using XCloud.Application.Service;
using XCloud.Core.Cache;
using XCloud.Core.IdGenerator;
using XCloud.Platform.Core.Application;
using XCloud.Platform.Core.Domain.Messenger;
using XCloud.Platform.Data.Database;

namespace XCloud.Platform.Application.Messenger.Service;

public interface IServerInstanceService : IXCloudApplicationService
{
    Task RegAsync(ServerInstanceDto dto);

    Task<string[]> QueryAllInstancesAsync();
}

public class ServerInstanceService : PlatformApplicationService, IServerInstanceService
{
    private readonly IPlatformRepository<SysServerInstance> _repository;

    public ServerInstanceService(IPlatformRepository<SysServerInstance> repository)
    {
        _repository = repository;
    }
    
    private async Task RegImplAsync(ServerInstanceDto dto)
    {
        var entity = await this._repository.QueryOneAsync(x => x.InstanceId == dto.InstanceId);

        if (entity == null)
        {
            entity = this.ObjectMapper.Map<ServerInstanceDto, SysServerInstance>(dto);

            entity.Id = this.GuidGenerator.CreateGuidString();
            entity.CreationTime = this.Clock.Now;

            await this._repository.InsertAsync(entity);
        }
        else
        {
            entity.ConnectionCount = dto.ConnectionCount;
            entity.PingTime = dto.PingTime;

            await this._repository.UpdateAsync(entity);
        }

        await this.CurrentUnitOfWork.SaveChangesAsync();
    }
    
    public async Task RegAsync(ServerInstanceDto dto)
    {
        if (string.IsNullOrWhiteSpace(dto.InstanceId))
            throw new ArgumentNullException(nameof(dto.InstanceId));

        await this.CacheProvider.GetOrSetAsync(async () =>
        {
            await this.RegImplAsync(dto);
            return string.Empty;
        }, new CacheOption<string>()
        {
            Key = $"im.server.instance.reg.{dto.InstanceId}",
            Expiration = TimeSpan.FromMinutes(1)
        });
    }

    public async Task<string[]> QueryAllInstancesAsync()
    {
        var db = await this._repository.GetDbContextAsync();

        var query = db.Set<SysServerInstance>().AsNoTracking();

        var time = this.Clock.Now.AddMinutes(-2);

        query = query.Where(x => x.PingTime > time);

        var data = await query.Select(x => x.InstanceId).Distinct().ToArrayAsync();

        return data;
    }
}