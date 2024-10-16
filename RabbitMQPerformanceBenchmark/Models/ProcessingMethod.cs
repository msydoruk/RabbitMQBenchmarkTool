namespace RabbitMQPerformanceBenchmark.Models
{
    public enum ProcessingMethod
    {
        Hidden = 0,
        GreedyLocator = 1,
        StrictLeafLocator = 2,
        NonLeafLevelLocator = 3,
        LowerLevelSignalLocator = 4
    }
}
