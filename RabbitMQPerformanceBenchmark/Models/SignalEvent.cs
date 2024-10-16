namespace RabbitMQPerformanceBenchmark.Models
{
    public class SignalEvent
    {
        public int AreaID { get; set; }

        public Guid SensorID { get; set; }

        public int ObjectID { get; set; }

        public string TagID { get; set; }

        public TagCategory Category { get; set; }

        public SignalEventType Type { get; set; }

        public ProcessingMethod Method { get; set; }

        public DateTime TimestampUtc { get; set; }

        public DateTime CurrentUtc { get; set; }

        public int? ExtraInspectorCount { get; set; }

        public string AdditionalEventDataJson { get; set; }
    }
}
