namespace XCloud.Platform.Application.Messenger.Configuration;

public class MessengerBuilder
{
    public IServiceCollection Services { get; }

    public MessengerBuilder(IServiceCollection services)
    {
        this.Services = services;
    }
}