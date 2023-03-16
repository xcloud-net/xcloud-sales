using System.Collections.Generic;
using XCloud.Core.Helper;
using XCloud.Platform.Application.Messenger.Exceptions;

namespace XCloud.Platform.Application.Messenger.Tasks;

public interface IMessengerTaskManager : IDisposable
{
    void StartTasks();
}

public class MessengerTaskManager : IMessengerTaskManager
{
    private readonly IReadOnlyList<IMessengerTask> _tasks;
    private readonly ILogger _logger;

    public MessengerTaskManager(IServiceProvider serviceProvider, ILogger<MessengerTaskManager> logger)
    {
        this._logger = logger;
        this._tasks = serviceProvider.GetServices<IMessengerTask>().ToArray();
    }

    private readonly CancellationTokenSource _cancellationTokenSource = new CancellationTokenSource();
    private CancellationToken CancellationToken => this._cancellationTokenSource.Token;

    private IReadOnlyList<Task> _threads;

    public void StartTasks()
    {
        if (this._threads != null)
            throw new MessengerException("messenger task already started");

        this._threads = this._tasks.Select(x => Task.Run(() => RunMessengerTaskAsync(x), CancellationToken)).ToArray();
    }

    private async Task RunMessengerTaskAsync(IMessengerTask task)
    {
        while (true)
        {
            if (this.CancellationToken.IsCancellationRequested)
            {
                break;
            }

            try
            {
                await task.ExecuteAsync(this.CancellationToken);
            }
            catch (Exception e)
            {
                this._logger.LogError(message: e.Message, exception: e);
            }

            await Task.Delay(task.Delay, CancellationToken);
        }
    }

    public void Dispose()
    {
        this._cancellationTokenSource.Cancel();

        if (ValidateHelper.IsNotEmptyCollection(this._threads))
        {
            var waitForThreads = Task.WhenAll(this._threads.ToArray());
            var waitForDelay = Task.Delay(TimeSpan.FromSeconds(10));

            Task.WhenAny(waitForThreads, waitForDelay);
        }

        this._cancellationTokenSource.Dispose();
    }
}