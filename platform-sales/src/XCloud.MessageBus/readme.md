# message bus

- masstransit
- rebus
- dotnet cap
- dapr（推荐）

## masstransit
```csharp
        public static void AddMasstransitKafkaProvider(this IServiceCollection services)
        {
            services.AddMassTransit(configure =>
            {
                configure.UsingInMemory();
                configure.AddRider(rider =>
                {
                    rider.UsingKafka((context, kafkaOption) =>
                    {
                        kafkaOption.Host("localhost:9091");

                    });
                });
            });
            throw new NotImplementedException();
        }
```