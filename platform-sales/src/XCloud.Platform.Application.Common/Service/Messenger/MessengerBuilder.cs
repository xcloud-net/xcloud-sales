namespace XCloud.Platform.Application.Common.Service.Messenger;

public class MessengerBuilder
{
    public IServiceCollection Services { get; }

    public MessengerBuilder(IServiceCollection services)
    {
        this.Services = services;
    }
}