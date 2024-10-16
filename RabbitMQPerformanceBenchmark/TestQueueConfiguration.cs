namespace RabbitMQPerformanceBenchmark
{
    public static class TestQueueConfiguration
    {
        public static string HostName = "localhost";

        public static string QueueName = "TestQueue";

        public static int TotalMessages = 10000;

        public static int MessageReceiveTimeoutMilliseconds = 1000;
    }
}
