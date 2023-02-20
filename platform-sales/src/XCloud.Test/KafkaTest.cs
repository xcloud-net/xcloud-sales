using Confluent.Kafka;
using Confluent.Kafka.Admin;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace XCloud.Test;

[TestClass]
public class KafkaTest
{
    //[TestMethod]
    public async Task TestKafka()
    {
        await Task.CompletedTask;

        var server = "localhost:9092";

        using IAdminClient admin = new AdminClientBuilder(new AdminClientConfig() { BootstrapServers = server }).Build();
        var groups = admin.ListGroups(TimeSpan.FromSeconds(5));
        var metadata = admin.GetMetadata(TimeSpan.FromSeconds(5));

        var testTopic = "test-topic";

        if (!metadata.Topics.Select(x => x.Topic).Contains(testTopic))
        {
            var topic = new TopicSpecification() { Name = testTopic };
            await admin.CreateTopicsAsync(new[] { topic }, new CreateTopicsOptions() { });
        }

        var config = new ProducerConfig()
        {
            BootstrapServers = server
        };

        new ConsumerConfig() { GroupId = "xx", BootstrapServers = server };
    }
}