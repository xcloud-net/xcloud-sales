# zookeeper

## client
```csharp
{
    /// <summary>
    /// 资料：邮箱搜索“zookeeper资料”
    /// </summary>
    public class ZooKeeperClient : IDisposable
    {
        protected readonly ILogger logger;

        protected bool IsDisposing = false;
        private ZooKeeper _client;
        //param
        protected readonly string _host;
        protected readonly TimeSpan _timeout;
        protected readonly Watcher _connection_status_watcher;
        //zk是否可用的信号量
        private readonly ManualResetEvent _client_lock = new ManualResetEvent(false);
        //创建zk的锁
        private readonly object _create_client_lock = new object();

        /// <summary>
        /// 链接成功
        /// </summary>
        public event Func<Task> OnConnectedAsync;

        /// <summary>
        /// 链接丢失，zk将自动重试链接
        /// </summary>
        public event Func<Task> OnUnConnectedAsync;

        /// <summary>
        /// session过期，zk将放弃链接
        /// </summary>
        public event Func<Task> OnSessionExpiredAsync;

        /// <summary>
        /// 捕获到异常
        /// </summary>
        public event Action<Exception> OnError;

        public ZooKeeperClient(ILogger logger, string host, TimeSpan? timeout = null)
        {
            this.logger = logger;
            this._host = host ?? throw new ArgumentNullException(nameof(host));
            this._timeout = timeout ?? TimeSpan.FromSeconds(30);
            //监控zk的状态
            this._connection_status_watcher = new ConnectionStatusWatcher(async status =>
            {
                this._client_lock.Reset();
                switch (status)
                {
                    case Watcher.Event.KeeperState.SyncConnected:
                    case Watcher.Event.KeeperState.ConnectedReadOnly:
                        //服务可用
                        this._client_lock.Set();
                        if (this.OnConnectedAsync != null) { await this.OnConnectedAsync.Invoke(); }
                        break;

                    case Watcher.Event.KeeperState.Disconnected:
                        //链接丢失，等待再次连接
                        if (this.OnUnConnectedAsync != null) { await this.OnUnConnectedAsync.Invoke(); }
                        break;

                    case Watcher.Event.KeeperState.Expired:
                        //session过期，重新创建客户端
                        if (this.OnSessionExpiredAsync != null) { await this.OnSessionExpiredAsync.Invoke(); }
                        break;

                    case Watcher.Event.KeeperState.AuthFailed:
                        //验证错误没必要再试尝试
                        this.Dispose();
                        //记录日志
                        var e = new AccessDenyException("zk auth验证失败，已经销毁所有链接");
                        this.logger.AddErrorLog(e.Message, e: e);
                        throw e;
                }
            });

            //这里注释掉，让用户手动调用
            //this.CreateClient();
        }

        public virtual void CreateClient()
        {
            if (this._client == null)
            {
                lock (this._create_client_lock)
                {
                    if (this._client == null)
                    {
                        this._client = new ZooKeeper(
                            this._host, (int)this._timeout.TotalMilliseconds,
                            this._connection_status_watcher);
                    }
                }
            }
        }

        public ZooKeeper Client { get => this.GetClientManager(); }

        /// <summary>
        /// 等待可用链接，默认30秒超时
        /// </summary>
        /// <returns></returns>
        public virtual ZooKeeper GetClientManager(TimeSpan? timeout = null)
        {
            try
            {
                //等待连上
                this._client_lock.WaitOneOrThrow(timeout ?? TimeSpan.FromSeconds(30), "无法链接zk");

                if (this._client == null) { throw new Exception("zookeeper client is not prepared"); }

                return this._client;
            }
            catch (KeeperException.ConnectionLossException e)
            {
                this.OnError?.Invoke(e);
                //链接断开
                throw new Exception("zk链接丢失", e);
            }
            catch (KeeperException.SessionExpiredException e)
            {
                this.OnError?.Invoke(e);
                //链接断开
                throw new Exception("zk会话丢失", e);
            }
            catch (Exception e)
            {
                this.OnError?.Invoke(e);
                throw;
            }
        }

        public virtual void CloseClient()
        {
            try
            {
                if (this._client != null)
                    Task.Run(async () => await this._client.closeAsync()).Wait();
            }
            catch (Exception e)
            {
                this.OnError?.Invoke(e);
                this.logger.AddErrorLog("关闭zk客户端失败", e: e);
            }
            finally
            {
                //关闭信号
                this._client_lock.Reset();
                this._client = null;
            }
        }

        public virtual void Dispose()
        {
            this.IsDisposing = true;

            this.CloseClient();
            this._client_lock.Dispose();
        }
    }
}
```

## alwaysOnClient

```csharp
{
    public class AlwaysOnZooKeeperClient : ZooKeeperClient
    {
        /// <summary>
        /// 尝试再次链接，也许还没连上
        /// </summary>
        public event Func<Task> OnRecconectingAsync;

        public AlwaysOnZooKeeperClient(ILogger logger, string host) : base(logger, host)
        {
            //只有session过期才重新创建client，否则等待client自动尝试重连
            this.OnSessionExpiredAsync += this.ReConnect;
        }

        protected async Task ReConnect()
        {
            if (this.IsDisposing)
            {
                //销毁的时候取消重试链接
                return;
            }

            this.CloseClient();
            this.CreateClient();

            if (this.OnRecconectingAsync != null)
                await this.OnRecconectingAsync.Invoke();
        }

        public override void Dispose()
        {
            base.Dispose();
        }
    }
}

```

## configuration provider

```csharp
        //config
        public static IConfigurationBuilder AddZookeeper(
            this IConfigurationBuilder builder,
            AlwaysOnZooKeeperClient client,
            CancellationToken cancellationToken,
            Func<ZKConfigurationOption, ZKConfigurationOption> config = null)
        {
            var option = new ZKConfigurationOption();
            if (config != null)
            {
                option = config.Invoke(option);
            }

            var zkConfigSource = new ZKConfigurationSource(option, client, cancellationToken);

            return builder.Add(zkConfigSource);
        }

            //source
            public sealed class ZKConfigurationSource : IConfigurationSource
            {
                private readonly IConfigurationProvider _provider;

                public ZKConfigurationSource(ZKConfigurationOption option, AlwaysOnZooKeeperClient client, CancellationToken cancellationToken)
                {
                    this._provider = new ZKConfigurationProvider(option, client);
                }

                public IConfigurationProvider Build(IConfigurationBuilder builder)
                {
                    return this._provider;
                }
            }

            //provider
{
    internal sealed class ZKConfigurationProvider : ConfigurationProvider, IConfigurationProvider
    {
        private IDictionary<string, string> PathData;

        private readonly ZKConfigurationOption _option;
        private readonly AlwaysOnZooKeeperClient _client;
        private readonly IJsonDataSerializer _serializer;
        private readonly Encoding _encoding;
        private readonly Watcher _node_watcher;

        public ZKConfigurationProvider(ZKConfigurationOption option, AlwaysOnZooKeeperClient client,
            IJsonDataSerializer serializeProvider = null,
            Encoding encoding = null)
        {
            this._option = option;
            this._client = client;
            this._serializer = serializeProvider;
            this._encoding = encoding ?? Encoding.UTF8;

            this._node_watcher = new CallBackWatcher(this.NodeWatchCallback);
        }

        public override void Load()
        {
            var base_path = this._option.BasePath;
            var max_deep = this._option.MaxDeep ?? 10;

            this.PathData = new Dictionary<string, string>();
            var node_history = new List<string>();

            async Task load_(string parent_path, string path, int deep)
            {
                var current_deep = deep;
                if (current_deep > max_deep)
                    return;

                var current_full_path = new string[] { parent_path, path }.AsZookeeperPath();

                if (node_history.Contains(current_full_path))
                    throw new ArgumentException("递归异常");
                node_history.Add(current_full_path);

                try
                {
                    var data = await this._client.Client.getDataAsync(current_full_path, watcher: null);
                    if (data?.Data?.Any() ?? false)
                    {
                        //当前节点有数据
                        var bs = data.Data;
                        var str_data = this._encoding.GetString(bs);
                        this.PathData[current_full_path] = str_data;
                    }

                    var children = await this._client.Client.getChildrenAsync(current_full_path);
                    if (children?.Children?.Any() ?? false)
                    {
                        var children_deep = current_deep + 1;
                        foreach (var child in children.Children)
                            //find next level
                            await load_(parent_path: current_full_path, path: child, deep: children_deep);
                    }
                }
                catch (Exception e)
                {
                    var context_instance = IocContext.Instance;
                    if (context_instance.Inited)
                    {
                        using (var s = context_instance.Scope())
                        {
                            var logger = s.ServiceProvider.ResolveLogger<ZKConfigurationProvider>();
                            logger.AddErrorLog(e.Message, e);
                        }
                    }
                }
            }

            var job = load_(parent_path: string.Empty, path: base_path, deep: 0);
            Task.Run(async () => await job).Wait();

            var dict = new Dictionary<string, string>();
            foreach (var m in this.PathData)
                dict[parse_path(m.Key)] = m.Value;

            this.Data = dict;
        }

        string parse_path(string zk_path) => string.Join(":", zk_path.Split('/', '\\').Where(x => x?.Length > 0));

        /// <summary>
        /// 节点改变会被通知到
        /// </summary>
        /// <param name="event"></param>
        /// <returns></returns>
        async Task NodeWatchCallback(WatchedEvent @event)
        {
            var base_path = this._option.BasePath;

            var path = @event.getPath();
            var type = @event.get_Type();

            switch (type)
            {
                case Watcher.Event.EventType.NodeChildrenChanged:
                case Watcher.Event.EventType.NodeDataChanged:
                case Watcher.Event.EventType.NodeDeleted:

                default: break;
            }

            await Task.CompletedTask;
        }

    }
}
```