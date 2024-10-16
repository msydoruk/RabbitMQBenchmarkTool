namespace RabbitMQPerformanceBenchmark.Models
{
    public class MessageQueueContent
    {
        public string MessageId { get; set; }

        public string MessageBody { get; set; }

        public string ReceiptHandle { get; set; }
    }
}
