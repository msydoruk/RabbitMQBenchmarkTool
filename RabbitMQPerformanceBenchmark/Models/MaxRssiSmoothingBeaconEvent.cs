namespace RabbitMQPerformanceBenchmark.Models
{
    public class MaxRssiSmoothingBeaconEvent
    {
        public int LocationID { get; set; }
        public Guid DeviceID { get; set; }
        public int EntityID { get; set; }
        public string BeaconID { get; set; }
        public BeaconType BeaconType { get; set; }
        public MaxRssiSmoothingBeaconEventType EventType { get; set; }
        public AlgorithmHandlerType AlgorithmHandlerType { get; set; }
        public DateTime EventTimeUtc { get; set; }
        public DateTime CurrentTimeUtc { get; set; }
        public int? AdditionalInspectorsCount { get; set; }
        public string ExtendedEventLogJson { get; set; }
    }
}
