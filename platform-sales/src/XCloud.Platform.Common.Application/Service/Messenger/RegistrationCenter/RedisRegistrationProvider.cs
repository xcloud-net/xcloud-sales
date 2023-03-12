using System.Threading.Tasks;
using FluentAssertions;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Timing;
using XCloud.Core.Json;

namespace XCloud.Platform.Common.Application.Service.Messenger.RegistrationCenter;

[ExposeServices(typeof(IRegistrationProvider))]
public class RedisRegistrationProvider : IRegistrationProvider, ISingletonDependency
{
    private readonly int expired_mins = 10;

    private readonly IRedisDatabaseSelector redisDatabaseSelector;
    private readonly IJsonDataSerializer messageSerializer;
    private readonly IRedisKeyManager redisKeyManager;
    private readonly IClock clock;

    public RedisRegistrationProvider(
        IClock clock,
        IRedisDatabaseSelector redisDatabaseSelector,
        IJsonDataSerializer messageSerializer,
        IRedisKeyManager redisKeyManager)
    {
        this.clock = clock;
        this.redisDatabaseSelector = redisDatabaseSelector;
        this.messageSerializer = messageSerializer;
        this.redisKeyManager = redisKeyManager;
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

        var user_reg_key = this.redisKeyManager.UserRegInfoKey(info.UserId);
        var device_reg_key = this.redisKeyManager.UserDeviceRegHashKey(info.DeviceType);

        var db = this.redisDatabaseSelector.Database;

        await db.HashSetAsync(user_reg_key, device_reg_key, data);

        //五分钟没有心跳就删除key
        await db.KeyExpireAsync(user_reg_key, TimeSpan.FromMinutes(this.expired_mins));
    }

    public async Task RegisterGroupInfoAsync(GroupRegistrationInfo info)
    {
        info.Should().NotBeNull();
        info.Payload.Should().NotBeNull();

        var key = this.redisKeyManager.GroupRegInfoKey(info.GroupId);
        var hash_key = this.redisKeyManager.GroupServerHashKey(info.ServerInstance);

        var db = this.redisDatabaseSelector.Database;

        var data = this.messageSerializer.SerializeToBytes(info.Payload);
        await db.HashSetAsync(key, hash_key, data);

        //五分钟没有心跳就删除key
        await db.KeyExpireAsync(key, TimeSpan.FromMinutes(this.expired_mins));
    }

    public async Task<string[]> GetUserServerInstancesAsync(string user_uid)
    {
        user_uid.Should().NotBeNullOrEmpty();

        var key = this.redisKeyManager.UserRegInfoKey(user_uid);

        var db = this.redisDatabaseSelector.Database;

        var entry = await db.HashGetAllAsync(key);

        var data = entry.Select(x => this.messageSerializer.DeserializeFromBytes<UserRegistrationInfoPayload>((byte[])x.Value)).ToArray();

        var reg_time_expired_ = this.clock.Now.AddMinutes(-this.expired_mins);

        var server_instance_id_arr = data.Where(x => x.PingTimeUtc > reg_time_expired_).Select(x => x.ServerInstanceId).Distinct().ToArray();

        return server_instance_id_arr;
    }

    public async Task RemoveRegisterInfoAsync(string user_uid, string device)
    {
        var user_reg_key = this.redisKeyManager.UserRegInfoKey(user_uid);
        var device_reg_key = this.redisKeyManager.UserDeviceRegHashKey(device);

        await this.redisDatabaseSelector.Database.HashDeleteAsync(user_reg_key, device_reg_key);
    }
}