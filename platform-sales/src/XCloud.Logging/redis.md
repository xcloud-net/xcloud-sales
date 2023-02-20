# logstash的redis input

```csharp
internal class ElkRedisLogger : ILogger
    {
        public ElkRedisLogger(string logger_name)
        {
            //
        }

        private readonly EmptyDisposable _disposer = new EmptyDisposable();

        public IDisposable BeginScope<TState>(TState state) => this._disposer;

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
        {
            throw new NotImplementedException();
        }

        public bool IsEnabled(LogLevel logLevel)
        {
            throw new NotImplementedException();
        }
    }
```

```csharp
    internal class ElkRedisProvider : ILoggerProvider
    {
        private readonly ConcurrentDictionary<string, ILogger> _loggers = new ConcurrentDictionary<string, ILogger>();

        private readonly Func<string, ILogger> _logger_getter;

        public ElkRedisProvider()
        {
            this._logger_getter = key => new ElkRedisLogger(key);
        }

        public ILogger CreateLogger(string categoryName)
        {
            var loggerName = categoryName ?? "default-logger";
            var logger = this._loggers.GetOrAdd(key: loggerName, valueFactory: this._logger_getter);
            return logger;
        }

        public void Dispose()
        {
            this._loggers.Clear();
        }
    }
```

```csharp
    static class RedisLoggingBuilderExtension
    {
        static ILoggingBuilder AddElkRedisPipline(this ILoggingBuilder builder)
        {
            builder.AddProvider(provider: new ElkRedisProvider());
            return builder;
        }
    }
```