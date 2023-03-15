using FluentAssertions;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Timing;
using XCloud.Core.Json;
using XCloud.Platform.Application.Messenger.Redis;

namespace XCloud.Platform.Application.Messenger.Registry;

[ExposeServices(typeof(IRegistrationProvider))]
public class RedisRegistrationProvider : IRegistrationProvider, ISingletonDependency
{
    private readonly int _expiredMins = 10;

    private readonly MessengerRedisClient redisDatabaseSelector;
    private readonly IJsonDataSerializer messageSerializer;
    private readonly IClock clock;

    public RedisRegistrationProvider(
        IClock clock,
        MessengerRedisClient redisDatabaseSelector,
        IJsonDataSerializer messageSerializer)
    {
        this.clock = clock;
        this.redisDatabaseSelector = redisDatabaseSelector;
        this.messageSerializer = messageSerializer;
    }

    /// <summary>
    /// userid=device:data
    /// groupid=serverinstanceid:data
    /// </summary>
    /// <param name="info"></param>
    /// <returns></returns>
    public async Task RegisterUserInfoAsync(UserRegistrationInfo info)
    {
        info.Should().NotBeNull();
        info.Payload.Should().NotBeNull();

        info.Payload.PingTimeUtc = this.clock.Now;

        var data = this.messageSerializer.SerializeToBytes(info.Payload);

        var userRegKey = this.redisDatabaseSelector.RedisKeyManager.UserRegInfoKey(info.UserId);
        var deviceRegKey = this.redisDatabaseSelector.RedisKeyManager.UserDeviceRegHashKey(info.DeviceType);

        var db = this.redisDatabaseSelector.Database;

        await db.HashSetAsync(userRegKey, deviceRegKey, data);

        //五分钟没有心跳就删除key
        await db.KeyExpireAsync(userRegKey, TimeSpan.FromMinutes(this._expiredMins));
    }

    public async Task<string[]> GetUserServerInstancesAsync(string userUid)
    {
        userUid.Should().NotBeNullOrEmpty();

        var key = this.redisDatabaseSelector.RedisKeyManager.UserRegInfoKey(userUid);

        var db = this.redisDatabaseSelector.Database;

        var entry = await db.HashGetAllAsync(key);

        var data = entry.Select(x =>
            this.messageSerializer.DeserializeFromBytes<UserRegistrationInfoPayload>((byte[])x.Value)).ToArray();

        var regTimeExpired = this.clock.Now.AddMinutes(-this._expiredMins);

        var serverInstanceIdArr = data.Where(x => x.PingTimeUtc > regTimeExpired).Select(x => x.ServerInstanceId)
            .Distinct().ToArray();

        return serverInstanceIdArr;
    }

    public async Task RemoveRegisterInfoAsync(string userUid, string device)
    {
        var userRegKey = this.redisDatabaseSelector.RedisKeyManager.UserRegInfoKey(userUid);
        var deviceRegKey = this.redisDatabaseSelector.RedisKeyManager.UserDeviceRegHashKey(device);

        await this.redisDatabaseSelector.Database.HashDeleteAsync(userRegKey, deviceRegKey);
    }
}