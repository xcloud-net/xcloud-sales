using FluentAssertions;
using StackExchange.Redis;
using System;
using XCloud.Core.DependencyInjection.ServiceWrapper;

namespace XCloud.Redis;

/// <summary>
/// https://stackexchange.github.io/StackExchange.Redis/Configuration
/// </summary>
public class RedisClient : ISingleInstanceService
{
    public string ConnectionString { get; }

    public ConnectionMultiplexer Connection { get; }

    public RedisClient(ConnectionMultiplexer con)
    {
        this.Connection = con ?? throw new ArgumentNullException(nameof(IConnectionMultiplexer));
    }

    static ConnectionMultiplexer __connect__(string connectionString)
    {
        connectionString.Should().NotBeNullOrEmpty();

        var connection = ConnectionMultiplexer.Connect(connectionString);

        return connection;
    }

    public RedisClient(string connectionString) : this(__connect__(connectionString))
    {
        this.ConnectionString = connectionString;
        this.ConnectionString.Should().NotBeNullOrEmpty();
    }

    public int DisposeOrder => int.MinValue;

    public void Dispose()
    {
        this.Connection?.Dispose();
    }
}