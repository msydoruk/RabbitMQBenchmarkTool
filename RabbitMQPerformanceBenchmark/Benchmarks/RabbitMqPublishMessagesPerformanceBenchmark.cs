using System.Text;
using BenchmarkDotNet.Attributes;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQPerformanceBenchmark.Models;

namespace RabbitMQPerformanceBenchmark.Benchmarks
{
    [MemoryDiagnoser]
    [IterationCount(10)] // Run each benchmark 10 times
    [WarmupCount(2)]     // Perform 2 warm-up iterations before measurement starts
    public class RabbitMqPublishMessagesPerformanceBenchmark
    {
        private const string QueueName = "TestQueue";
        private const int TotalMessages = 10000;
        private IModel _channel;
        private IConnection _connection;

        [GlobalSetup]
        public void Setup()
        {
            var factory = new ConnectionFactory() { HostName = "localhost" };
            _connection = factory.CreateConnection();
            _channel = _connection.CreateModel();

            // Ensure the queue exists before testing
            // _channel.QueueDeclare(queue: QueueName, durable: false, exclusive: false, autoDelete: false, arguments: null);
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
            var beaconEvent = new MaxRssiSmoothingBeaconEvent
            {
                LocationID = 56,
                DeviceID = new Guid("ACD70D00-30DD-46FA-9E7B-B00300FF9BF7"),
                EntityID = 3,
                BeaconID = "E101B392ADA32224231605EF5877491700033FD3",
                BeaconType = BeaconType.Asset,
                EventType = MaxRssiSmoothingBeaconEventType.Position,
                AlgorithmHandlerType = AlgorithmHandlerType.GreedinessLocationHandler,
                EventTimeUtc = DateTime.UtcNow,
                CurrentTimeUtc = DateTime.UtcNow,
                AdditionalInspectorsCount = 4,
                ExtendedEventLogJson = "{}"
            };

            string messageBody = JsonConvert.SerializeObject(beaconEvent);
            var body = Encoding.UTF8.GetBytes(messageBody);

            for (int i = 0; i < TotalMessages; i++)
            {
                _channel.BasicPublish(exchange: "", routingKey: QueueName, basicProperties: null, body: body);
            }
        }
    }
}
