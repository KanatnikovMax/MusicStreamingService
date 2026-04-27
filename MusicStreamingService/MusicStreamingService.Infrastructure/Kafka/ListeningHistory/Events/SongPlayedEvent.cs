namespace MusicStreamingService.Infrastructure.Kafka.ListeningHistory.Events;

public class SongPlayedEvent
{
    public Guid EventId { get; set; }
    public Guid UserId { get; set; }
    public Guid SongId { get; set; }
    public DateTime ListenedAtUtc { get; set; }
}
