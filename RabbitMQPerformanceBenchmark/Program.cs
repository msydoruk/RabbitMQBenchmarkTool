using BenchmarkDotNet.Running;
using RabbitMQPerformanceBenchmark.Benchmarks;

try
{
    BenchmarkRunner.Run<RabbitMqPublishMessagesPerformanceBenchmark>();
}
catch (Exception exception)
{
    Console.WriteLine($"An error occurred: {exception.Message}");
    Console.WriteLine(exception.StackTrace);
}
finally
{
    Console.WriteLine("Press any key to exit...");
    Console.ReadLine();
}