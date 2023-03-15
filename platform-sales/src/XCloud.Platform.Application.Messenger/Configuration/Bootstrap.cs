using XCloud.Core.Configuration.Builder;
using XCloud.Platform.Application.Messenger.Server;

namespace XCloud.Platform.Application.Messenger.Configuration;

public static class Bootstrap
{
    public static MessengerBuilder AddMessengerServer(this IXCloudBuilder builder)
    {
        return new MessengerBuilder(builder.Services);
    }

    public static MessengerBuilder AddDefaultHubProvider(this MessengerBuilder builder)
    {
        var instanceId = $"server:{Guid.NewGuid().ToString()}@{System.Net.Dns.GetHostName()}";
        builder.Services.AddSingleton(provider => new MessengerServer(provider, instanceId));
        builder.Services.AddSingleton<IMessengerServer>(provider => provider.GetRequiredService<MessengerServer>());
        return builder;
    }
}