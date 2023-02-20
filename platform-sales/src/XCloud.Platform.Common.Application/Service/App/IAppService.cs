using System.Threading.Tasks;
using StackExchange.Redis;
using XCloud.Core.Application;
using XCloud.Platform.Core.Application;
using XCloud.Redis;

namespace XCloud.Platform.Common.Application.Service.App;

public interface IAppService : IXCloudApplicationService
{
    Task FlushAppInformation(Models.App[] apps);
}

public class AppService : PlatformApplicationService, IAppService
{
    private readonly RedisClient _redisClient;
    private readonly IDatabase _database;

    public AppService(IServiceProvider serviceProvider, RedisClient redisClient)
    {
        this._redisClient = redisClient;
        this._database = this._redisClient.Connection.GetDatabase(3);
    }

    private string RedisHashKey => "__app_registry__";

    public async Task FlushAppInformation(Models.App[] apps)
    {
        var all = await this._database.HashGetAllAsync(this.RedisHashKey);
    }
}