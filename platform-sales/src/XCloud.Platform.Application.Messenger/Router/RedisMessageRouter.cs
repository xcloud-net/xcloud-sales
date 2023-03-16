using FluentAssertions;
using Volo.Abp.DependencyInjection;
using XCloud.Core.Extension;
using XCloud.Core.Json;
using XCloud.Platform.Application.Messenger.Message;
using XCloud.Platform.Application.Messenger.Redis;

namespace XCloud.Platform.Application.Messenger.Router;

[ExposeServices(typeof(IMessageRouter))]
public class RedisMessageRouter : IMessageRouter, ISingletonDependency
{
    private readonly ILogger _logger;
    private readonly MessengerRedisClient _redisDatabaseSelector;
    private readonly IJsonDataSerializer _messageSerializer;
    private readonly CancellationTokenSource _tokenSource = new CancellationTokenSource();

    private Task _refreshKeyTask;
    private Task _consumeTask;

    public RedisMessageRouter(MessengerRedisClient redisDatabaseSelector, IJsonDataSerializer messageSerializer, ILogger<RedisMessageRouter> logger)
    {
        this._redisDatabaseSelector = redisDatabaseSelector;
        this._messageSerializer = messageSerializer;
        this._logger = logger;
    }

    public void Dispose()
    {
        this._tokenSource.Cancel();
    }

    public async Task RouteToServerInstance(string key, MessageDto data)
    {
        var bs = this._messageSerializer.SerializeToBytes(data);
        await this._redisDatabaseSelector.Database.ListLeftPushAsync(key, bs);
    }

    private async Task refresh_key(string key)
    {
        var db = this._redisDatabaseSelector.Database;
        while (true && !this._tokenSource.IsCancellationRequested)
        {
            try
            {
                await db.KeyExpireAsync(key, TimeSpan.FromHours(6));
                await Task.Delay(TimeSpan.FromSeconds(60), this._tokenSource.Token);
            }
            catch (Exception e)
            {
                this._logger.AddErrorLog(e.Message, e);
            }
        }
        this._logger.LogInformation("结束刷新key");
    }

    public async Task SubscribeMessageEndpoint(string key, Func<MessageDto, Task> callback)
    {
        async Task Consume()
        {
            while (true && !this._tokenSource.IsCancellationRequested)
            {
                try
                {
                    var res = await this._redisDatabaseSelector.Database.ListRightPopAsync(key);
                    if (!res.HasValue)
                    {
                        await Task.Delay(TimeSpan.FromMilliseconds(100), this._tokenSource.Token);
                        continue;
                    }
                    var data = this._messageSerializer.DeserializeFromBytes<MessageDto>((byte[])res);
                    await callback.Invoke(data);
                }
                catch (Exception e)
                {
                    this._logger.AddErrorLog(e.Message, e);
                    await Task.Delay(TimeSpan.FromMilliseconds(100), this._tokenSource.Token);
                }
            }
        }

        (this._consumeTask == null && this._refreshKeyTask == null).Should().BeTrue();

        this._consumeTask = Task.Run(Consume, this._tokenSource.Token);
        this._refreshKeyTask = Task.Run(() => this.refresh_key(key), this._tokenSource.Token);
        this._logger.LogInformation("开始订阅，开始刷新key");
        await Task.CompletedTask;
    }
}