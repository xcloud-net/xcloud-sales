namespace XCloud.Platform.Application.Messenger.Tasks;

public interface IMessengerTask : IDisposable
{
    TimeSpan Delay { get; }
    Task ExecuteAsync(CancellationToken cancellationToken);
}