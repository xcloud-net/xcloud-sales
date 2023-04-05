using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using StackExchange.Redis;
using XCloud.Application.Service;
using XCloud.Platform.Application.Common.Service.Apps.Models;
using XCloud.Platform.Core.Application;
using XCloud.Redis;

namespace XCloud.Platform.Application.Common.Service.Apps;

public interface IAppService : IXCloudApplicationService
{
    Task FlushAppInformation(App[] apps);

    Task<App[]> QueryAllAsync();
}

public class AppService : PlatformApplicationService, IAppService
{
    private readonly IDatabase _database;
    private readonly IWebHostEnvironment _webHostEnvironment;

    public AppService(RedisClient redisClient, IWebHostEnvironment webHostEnvironment)
    {
        _webHostEnvironment = webHostEnvironment;
        this._database = redisClient.Connection.GetDatabase(3);
    }

    public async Task<App[]> QueryAllAsync()
    {
        var path = Path.Combine(this._webHostEnvironment.ContentRootPath, "app.json");
        if (!File.Exists(path))
            throw new FileNotFoundException($"file not exist:{path}");

        var json = await File.ReadAllTextAsync(path);

        var apps = this.JsonDataSerializer.DeserializeFromString<App[]>(json);

        return apps;
    }

    private string RedisHashKey => "__app_registry__";

    public async Task FlushAppInformation(App[] apps)
    {
        var all = await this._database.HashGetAllAsync(this.RedisHashKey);
    }
}