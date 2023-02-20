# rabbitmq

## 死信实现延迟队列
> 发送消息到队列a，规定时间内没有被消费，则转发到死信队列
```csharp
        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        /// <param name="ttl">过期时间(秒)</param>
        /// <param name="queryName"></param>
        /// <param name="routerKey"></param>
        public virtual void PushDelyMessage(object message, int ttl, string queryName)
        {
            queryName ??= _options.QueryName;
            var delayworkexchange = _options.DelayWorkExchangeName; // dead letter exchange
            var delayexchange = _options.DelayExchangeName;

            if (string.IsNullOrWhiteSpace(delayworkexchange) || string.IsNullOrWhiteSpace(delayexchange))
            {
                throw new Exception("没有设置死信队列交换机");
            }

            var queueArgs = new Dictionary<string, object>
            {
                {"x-dead-letter-exchange", delayworkexchange},
                {"x-message-ttl", 2 * 60 * 60 * 1000} // 默认设置2小时过期
            };

            // TODO: add router-key will throw err??why??
            // if (routerKey != null)
            //     queueArgs.Add("x-dead-letter-routing-key", routerKey);

            _channel.ExchangeDeclare(delayexchange, "direct");
            _channel.QueueDeclare(queryName, durable: true, exclusive: false, autoDelete: false, arguments: queueArgs);
            _channel.QueueBind(queryName, delayexchange, routingKey: string.Empty, arguments: null);

            var sendBytes = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(message, new JsonSerializerSettings
                {
                    ReferenceLoopHandling = ReferenceLoopHandling.Ignore
                }
            ));

            var properties = _channel.CreateBasicProperties();
            properties.Expiration = (ttl * 1000).ToString(); //设置TTL为20000毫秒
            properties.Timestamp = new AmqpTimestamp(DateTimeOffset.UtcNow.UtcTicks);
            properties.MessageId = Guid.NewGuid().ToString("N");

            _channel.BasicPublish(delayexchange, string.Empty, properties, sendBytes);
        }
```

## create connection factory
```csharp
                var factory = new ConnectionFactory
                {
                    HostName = host_port[0],
                    Port = int.Parse(host_port[1]),
                    UserName = configuration.UserName,
                    Password = configuration.Password,
                    AutomaticRecoveryEnabled = true,
                    //下面这个配置不改为true，那么异步consumer不生效（！！！）
                    DispatchConsumersAsync = true,
                };
```

## route binding

```csharp
        {
            /*
             创建topic用于分发消息
             创建fanout用于广播消息
             部分需要广播的消息通过topic的routing key转发到fanout exchange里
             */

            var factory = new ConnectionFactory();
            var con = factory.CreateConnection();

            var channel = con.CreateModel();

            channel.ExchangeDeclare("shared-topic", "topic", durable: true, autoDelete: false);
            channel.ExchangeDeclare("user-group-change-broadcast", "fanout", durable: true, autoDelete: false);

            channel.ExchangeBind(destination: "user-group-change-broadcast", source: "shared-topic", routingKey: "user-group-change");



            var queue = channel.QueueDeclare(exclusive: true);
            channel.QueueBind(queue.QueueName, "user-group-change-broadcast", null);

            channel.BasicConsume(queue.QueueName, autoAck: true, null);

            channel.BasicPublish("shared-topic", "user-group-change", body: new byte[] { });
        }


//------------------------
        {
            //exchange
            this._channel.ExchangeDeclare(exchange: ConstConfig.ExchangeName,
                type: "direct",
                durable: true,
                autoDelete: false);

            //queue
            this._channel.QueueDeclare(queue: this.Settings.QueueName,
                //持久化
                durable: true,
                //排他，client断开链接自动删除queue
                exclusive: false,
                autoDelete: false);

            if (this.Settings.ConcurrencySize != null)
            {
                var size = this.Settings.ConcurrencySize.Value;
                this._channel.BasicQos(prefetchSize: 0, prefetchCount: size, global: false);
            }

            //route
            var args = new Dictionary<string, object>();
            this._channel.RouteFromExchangeToQueue(
                exchange: ConstConfig.ExchangeName,
                queue: this.Settings.QueueName,
                routing_key: Settings.QueueName,
                args: args);
        }

```

## publish

```csharp
{
    /// <summary>
    /// send a message
    /// 事务（txselect）和confirm模式（confirm）都可以用来确认消息，但是后者效率高
    /// 
    /// 事务确实可以判断producer向Broker发送消息是否成功，只有Broker接受到消息，才会commit，
    /// 但是使用事务机制的话会降低RabbitMQ的性能，那么有没有更好的方法既能保障producer知道消息已经正确送到，
    /// 又能基本上不带来性能上的损失呢？从AMQP协议的层面看是没有更好的方法，但是RabbitMQ提供了一个更好的方案，
    /// 即将channel信道设置成confirm模式。
    /// </summary>
    public class RabbitMqProducer : IRabbitMqProducer
    {
        private readonly IConnection _connection;
        private readonly IJsonDataSerializer _serializer;

        public RabbitMqProducer(RabbitConnectionWrapper connectionWrapper, IJsonDataSerializer _serializer)
        {
            this._connection = connectionWrapper.Connection;
            this._serializer = _serializer;
        }

        IBasicProperties __create_basic_properties__(IModel _channel, SendMessageOption option)
        {
            option.Should().NotBeNull();

            var basicProperties = _channel.CreateBasicProperties();
            var header = new Dictionary<string, object>();

            //参数
            if (!string.IsNullOrWhiteSpace(option.Properties))
            {
                header.AddDict(option.Properties.ToDictionary_(x => x.Key, x => x.Value));
            }
            //延迟
            if (option.Delay != null)
            {
                header["x-delay"] = Math.Abs((long)option.Delay.Value.TotalMilliseconds);
            }
            //持久化
            if (option.Persistent)
            {
                basicProperties.DeliveryMode = (byte)2;
                basicProperties.Persistent = true;
            }
            else
            {
                basicProperties.DeliveryMode = (byte)1;
                basicProperties.Persistent = false;
            }
            //优先级
            if (option.Priority != null)
            {
                basicProperties.Priority = (byte)option.Priority.Value;
            }

            //headers
            basicProperties.Headers = header;

            return basicProperties;
        }

        public void SendMessage<T>(string exchange, string routeKey, T message, SendMessageOption option = null)
        {
            option ??= new SendMessageOption() { };

            using var _channel = this._connection.CreateModel();

            var bs = this._serializer.SerializeToBytes(message);

            var props = this.__create_basic_properties__(_channel, option);

            using var confirm = new ConfirmOrNot(_channel, option.Confirm, option.ConfirmTimeout);
            try
            {
                _channel.BasicPublish(exchange: exchange,
                  routingKey: routeKey,
                  basicProperties: props,
                  body: bs);
            }
            catch
            {
                confirm.DontConfirmAnyMore();
                throw;
            }
        }


#if DEBUG
        [Obsolete]
        void TransactionTest()
        {
            using var _channel = this._connection.CreateModel();
            _channel.TxSelect();
            try
            {
                //publish
                //publish
                _channel.TxCommit();
            }
            catch
            {
                _channel.TxRollback();
            }
        }
#endif
    }
}

```

## consumer

```csharp
            var _consumer = new AsyncEventingBasicConsumer(this._channel);

            //注册消费事件
            _consumer.Received += this.__on_message_recieved__;

            var args = new Dictionary<string, object>();

            var consumer_tag = this._channel.BasicConsume(consumer: _consumer,
                queue: this.Settings.QueueName, autoAck: this.Settings.AutoAck,
                arguments: args);

            consumer_tag.Should().NotBeNullOrEmpty();
```

## consume in batch

```csharp
    {
        static async Task<bool> __sleep__(IModel _channel, AsyncCircuitBreakerPolicy breaker, ILogger logger)
        {
            var _continue = false;
            if (!_channel.IsOpen)
            {
                try
                {
                    await breaker.ExecuteAsync(() =>
                    {
                        logger.AddErrorLog("rabbitmq消费异常：channel已经关闭，等待恢复");

                        throw new Exception("错误日志熔断，避免写入太多日志");
                    });
                }
                catch
                {
                    //do nothing
                }
                await Task.Delay(TimeSpan.FromSeconds(5));
                _continue = true;
            }
            return _continue;
        }

        /// <summary>
        /// 3年或者10万公里逻辑
        /// </summary>
        /// <returns></returns>
        public static async IAsyncEnumerable<IReadOnlyCollection<BasicGetResult>> __batch_message__(
            this IModel _channel,
            string QueueName, bool AutoAck,
            int BatchSize, TimeSpan BatchTimeout,
            ILogger logger,
            CancellationTokenSource cancellationToken)
        {
            var batch_data = new List<BasicGetResult>();

            var breaker = Policy.Handle<Exception>().AdvancedCircuitBreakerAsync(
                failureThreshold: 3,
                samplingDuration: TimeSpan.FromMinutes(1),
                minimumThroughput: 1,
                durationOfBreak: TimeSpan.FromMinutes(1));

            var cursor = DateTime.UtcNow;

            bool Timeout(DateTime now) => (cursor + BatchTimeout) < now;
            while (true)
            {
                if (cancellationToken.IsCancellationRequested)
                {
                    break;
                }

                var _continue = await __sleep__(_channel, breaker, logger);
                if (_continue)
                {
                    continue;
                }

                //从队列中读取数据放入列表
                var data = _channel.BasicGet(QueueName, autoAck: AutoAck);
                if (data == null)
                {
                    //there is no data in queue,await 100 miliseconds
                    await Task.Delay(TimeSpan.FromMilliseconds(100));
                    continue;
                }
                batch_data.Add(data);

                //如果当前列表里的数据满足条件就yield
                var now = DateTime.UtcNow;
                if ((batch_data.Count >= BatchSize || Timeout(now)) && batch_data.Any())
                {
                    //consume
                    var d = batch_data.AsReadOnly();
                    yield return d;
                    //batch consuming finished ,reset cursor
                    batch_data = new List<BasicGetResult>() { };
                    cursor = now;
                }
            }
        }
    }
}
```

## confirm

```csharp
{
    /// <summary>
    /// 出scope自动确认消息，无法确认就抛错
    /// 
    /// 事务（txselect）和confirm模式（confirm）都可以用来确认消息，但是后者效率高
    /// 
    /// 事务确实可以判断producer向Broker发送消息是否成功，只有Broker接受到消息，才会commit，
    /// 但是使用事务机制的话会降低RabbitMQ的性能，那么有没有更好的方法既能保障producer知道消息已经正确送到，
    /// 又能基本上不带来性能上的损失呢？从AMQP协议的层面看是没有更好的方法，但是RabbitMQ提供了一个更好的方案，
    /// 即将channel信道设置成confirm模式。
    /// </summary>
    public class ConfirmOrNot : IDisposable
    {
        private readonly IModel _channel;
        private readonly TimeSpan? _confirm_timeout;
        private bool _confirm { get; set; }

        public ConfirmOrNot(IModel channel, bool confirm, TimeSpan? confirm_timeout)
        {
            this._channel = channel ?? throw new ArgumentNullException(nameof(channel));
            this._confirm = confirm;
            this._confirm_timeout = confirm_timeout;

            if (!this._confirm)
                return;

            this._channel.ConfirmSelect();
        }

        /// <summary>
        /// 如果publish抛异常了，就没必要confirm了。confirm会浪费时间
        /// </summary>
        public void DontConfirmAnyMore() => this._confirm = false;

        public void Dispose()
        {
            if (this._confirm)
            {
                try
                {
                    if (this._confirm_timeout == null)
                        this._channel.WaitForConfirmsOrDie();
                    else
                        this._channel.WaitForConfirmsOrDie(this._confirm_timeout.Value);
                }
                catch (Exception e)
                {
                    throw new TimeoutException("rabbitmq确认消息失败", e);
                }
            }
        }
    }
}
```