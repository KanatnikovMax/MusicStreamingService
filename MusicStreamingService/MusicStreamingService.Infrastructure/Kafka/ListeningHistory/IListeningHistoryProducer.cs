namespace MusicStreamingService.Infrastructure.Kafka.ListeningHistory;

public interface IListeningHistoryProducer
{
    Task ProduceSongPlayedAsync(Guid userId, Guid songId, DateTime listenedAtUtc, CancellationToken cancellationToken);
}
