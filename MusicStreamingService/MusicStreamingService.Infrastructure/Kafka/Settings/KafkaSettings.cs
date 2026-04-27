namespace MusicStreamingService.Infrastructure.Kafka.Settings;

public class KafkaSettings
{
    public string BootstrapServers { get; set; }
    public string ListeningHistoryTopic { get; set; }
}
