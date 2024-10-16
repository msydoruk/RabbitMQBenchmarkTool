using System.Text;
using BenchmarkDotNet.Attributes;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQPerformanceBenchmark.Models;

namespace RabbitMQPerformanceBenchmark.Benchmarks
{
    [MemoryDiagnoser]
    public class RabbitMqPublishMessagesPerformanceBenchmark
    {
        private IModel _channel;
        private IConnection _connection;

        [GlobalSetup]
        public void Setup()
        {
            var factory = new ConnectionFactory() { HostName = TestQueueConfiguration.HostName };
            _connection = factory.CreateConnection();
            _channel = _connection.CreateModel();
        }

        [GlobalCleanup]
        public void Cleanup()
        {
            _channel?.Dispose();
            _connection?.Dispose();
        }

        // Benchmark for publishing messages to RabbitMQ
        [Benchmark]
        public void PublishMessages()
        {
            var beaconEvent = new SignalEvent
            {
                AreaID = 56,
                SensorID = Guid.NewGuid(),
                ObjectID = 3,
                TagID = Guid.NewGuid().ToString(),
                Category = TagCategory.Equipment,
                Type = SignalEventType.LocationUpdate,
                Method = ProcessingMethod.StrictLeafLocator,
                TimestampUtc = DateTime.UtcNow,
                CurrentUtc = DateTime.UtcNow,
                ExtraInspectorCount = 4,
                AdditionalEventDataJson = "{}"
            };

            string messageBody = JsonConvert.SerializeObject(beaconEvent);
            var body = Encoding.UTF8.GetBytes(messageBody);

            for (int i = 0; i < TestQueueConfiguration.TotalMessages; i++)
            {
                _channel.BasicPublish(exchange: "", routingKey: TestQueueConfiguration.QueueName, basicProperties: null, body: body);
            }
        }
    }
}
