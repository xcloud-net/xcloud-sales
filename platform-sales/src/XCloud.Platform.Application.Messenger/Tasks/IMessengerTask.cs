namespace XCloud.Platform.Application.Messenger.Tasks;

public interface IMessengerTask
{
    TimeSpan Delay { get; }
    
    Task ExecuteAsync(CancellationToken cancellationToken);
}