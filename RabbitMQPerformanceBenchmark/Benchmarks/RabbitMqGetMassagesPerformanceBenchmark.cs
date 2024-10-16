using System.Text;
using BenchmarkDotNet.Attributes;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using RabbitMQPerformanceBenchmark.Models;

namespace RabbitMQPerformanceBenchmark.Benchmarks
{
    [MemoryDiagnoser]
    [IterationCount(10)] // Run each benchmark 10 times
    [WarmupCount(2)]     // Perform 2 warm-up iterations before measurement starts
    public class RabbitMqGetMassagesPerformanceBenchmark
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

        // Benchmark for BasicConsume with 1000 messages per batch
        [Benchmark]
        public async Task BenchmarkBasicConsume1000()
        {
            await ConsumeAllMessagesInBatches(1000);
        }

        // Benchmark for BasicConsume with 100 messages per batch
        [Benchmark]
        public async Task BenchmarkBasicConsume100()
        {
            await ConsumeAllMessagesInBatches(100);
        }

        // Benchmark for BasicGet with 1000 messages per batch
        [Benchmark]
        public void BenchmarkBasicGet1000()
        {
            GetAllMessages(1000);
        }

        // Benchmark for BasicGet with 100 messages per batch
        [Benchmark]
        public void BenchmarkBasicGet100()
        {
            GetAllMessages(100);
        }

        // Consume all messages in batches using BasicConsume
        private async Task ConsumeAllMessagesInBatches(int batchSize)
        {
            var totalMessagesReceived = 0;

            while (totalMessagesReceived < TestQueueConfiguration.TotalMessages)
            {
                var receivedMessages = await GetMessagesInBatchWithTimeout(TestQueueConfiguration.QueueName, batchSize);
                totalMessagesReceived += receivedMessages.Count;

                Console.WriteLine($"Received {receivedMessages.Count} messages in batch. Total received: {totalMessagesReceived}");
            }
        }

        // Get all messages using BasicGet
        private void GetAllMessages(int batchSize)
        {
            var totalMessagesReceived = 0;

            while (totalMessagesReceived < TestQueueConfiguration.TotalMessages)
            {
                var receivedMessages = GetMessagesWithBasicGet(batchSize);
                totalMessagesReceived += receivedMessages.Count;

                Console.WriteLine($"Received {receivedMessages.Count} messages in batch. Total received: {totalMessagesReceived}");
            }
        }

        // Method to fetch messages using BasicGet
        private IList<MessageQueueContent> GetMessagesWithBasicGet(int batchSize)
        {
            var messages = new List<MessageQueueContent>();

            for (int i = 0; i < batchSize; i++)
            {
                var result = _channel.BasicGet(queue: TestQueueConfiguration.QueueName, autoAck: false);
                if (result != null)
                {
                    var messageBody = Encoding.UTF8.GetString(result.Body.ToArray());
                    var messageQueueContent = new MessageQueueContent
                    {
                        MessageBody = messageBody,
                        ReceiptHandle = result.DeliveryTag.ToString()
                    };

                    messages.Add(messageQueueContent);
                    _channel.BasicAck(result.DeliveryTag, multiple: false);
                }
            }

            return messages;
        }

        // Method to fetch messages using BasicConsume with a timeout
        public async Task<IList<MessageQueueContent>> GetMessagesInBatchWithTimeout(string queueName, int maxBatchSize)
        {
            var messages = new List<MessageQueueContent>();
            var consumer = new EventingBasicConsumer(_channel);

            var taskCompletionSource = new TaskCompletionSource<IList<MessageQueueContent>>();
            var inactivityTimer = new System.Timers.Timer(TestQueueConfiguration.MessageReceiveTimeoutMilliseconds) { AutoReset = false };

            var consumerTag = Guid.NewGuid().ToString();
            var lockObject = new object();

            bool isProcessingCompleted = false;

            void CompleteConsumerProcessing()
            {
                lock (lockObject)
                {
                    if (isProcessingCompleted)
                    {
                        return;
                    }

                    isProcessingCompleted = true;

                    try
                    {
                        _channel.BasicCancel(consumerTag);
                    }
                    catch (Exception)
                    {
                        // ignored
                    }

                    if (inactivityTimer.Enabled)
                    {
                        inactivityTimer.Stop();
                        inactivityTimer.Dispose();
                    }

                    if (!taskCompletionSource.Task.IsCompleted)
                    {
                        taskCompletionSource.TrySetResult(messages);
                    }
                }
            }

            inactivityTimer.Elapsed += (sender, args) =>
            {
                CompleteConsumerProcessing();
            };

            inactivityTimer.Start();

            consumer.Received += (model, eventArgs) =>
            {
                lock (lockObject)
                {
                    if (isProcessingCompleted)
                    {
                        return;
                    }

                    inactivityTimer.Stop();

                    var messageBody = Encoding.UTF8.GetString(eventArgs.Body.ToArray());
                    var messageQueueContent = new MessageQueueContent
                    {
                        MessageBody = messageBody,
                        ReceiptHandle = eventArgs.DeliveryTag.ToString()
                    };

                    messages.Add(messageQueueContent);

                    if (messages.Count >= maxBatchSize)
                    {
                        CompleteConsumerProcessing();
                    }
                    else
                    {
                        inactivityTimer.Start();
                    }
                }
            };

            _channel.BasicQos(prefetchSize: 0, prefetchCount: (ushort)maxBatchSize, global: false);
            _channel.BasicConsume(queue: queueName, autoAck: false, consumer: consumer, consumerTag: consumerTag);

            var result = await taskCompletionSource.Task.ConfigureAwait(false);

            return result;
        }
    }
}
