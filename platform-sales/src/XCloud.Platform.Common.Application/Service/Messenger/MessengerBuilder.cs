namespace XCloud.Platform.Common.Application.Service.Messenger;

public class MessengerBuilder
{
    public IServiceCollection Services { get; }

    public MessengerBuilder(IServiceCollection services)
    {
        this.Services = services;
    }
}