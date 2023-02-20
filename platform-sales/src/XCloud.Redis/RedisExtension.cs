using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using StackExchange.Redis;
using System;

namespace XCloud.Redis;

public static class RedisExtension
{
    public static IDatabase SelectDatabase(this IConnectionMultiplexer con, int? db)
    {
        var res = db == null ? con.GetDatabase() : con.GetDatabase(db.Value);

        return res;
    }

    public static RedisClient GetRedisClient(this IServiceCollection collection, bool nullable = false)
    {
        var res = collection.GetSingletonInstanceOrNull<RedisClient>();
        if (!nullable)
        {
            res.Should().NotBeNull("请先注册redis链接");
        }
        return res;
    }

    public static string GetRedisConnectionString(this IConfiguration config)
    {
        var section = config.GetSection("app").GetSection("redis");

        if (section.Exists())
        {
            var host = section["host"];
            var port = section["port"];

            if (string.IsNullOrEmpty(host) || string.IsNullOrEmpty(port))
                throw new ArgumentNullException(nameof(GetRedisConnectionString));

            var connectionString = $"{host}:{port}";

            return connectionString;
        }
        else
        {
            var abpRedisConnectionString = config["Redis:Configuration"];

            abpRedisConnectionString.Should().NotBeNullOrEmpty(nameof(GetRedisConnectionString));

            return abpRedisConnectionString;
        }
    }
}