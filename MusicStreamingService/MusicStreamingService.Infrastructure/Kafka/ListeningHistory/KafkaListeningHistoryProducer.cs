using System.Text.Json;
using Confluent.Kafka;
using Microsoft.Extensions.Logging;
using MusicStreamingService.Infrastructure.Kafka.Settings;

namespace MusicStreamingService.Infrastructure.Kafka.ListeningHistory;

public class KafkaListeningHistoryProducer : IListeningHistoryProducer, IDisposable
{
    private readonly KafkaSettings _kafkaSettings;
    private readonly ILogger<KafkaListeningHistoryProducer> _logger;
    private readonly IProducer<string, string> _producer;

    public KafkaListeningHistoryProducer(KafkaSettings settings, ILogger<KafkaListeningHistoryProducer> logger)
    {
        _kafkaSettings = settings;
        _logger = logger;
        _producer = new ProducerBuilder<string, string>(new ProducerConfig
        {
            BootstrapServers = _kafkaSettings.BootstrapServers
        }).Build();
    }

    public async Task ProduceSongPlayedAsync(
        Guid userId,
        Guid songId,
        DateTime listenedAtUtc,
        CancellationToken cancellationToken)
    {
        var eventPayload = new Events.SongPlayedEvent
        {
            EventId = Guid.NewGuid(),
            UserId = userId,
            SongId = songId,
            ListenedAtUtc = listenedAtUtc
        };

        var message = new Message<string, string>
        {
            Key = userId.ToString(),
            Value = JsonSerializer.Serialize(eventPayload)
        };

        var deliveryResult = await _producer.ProduceAsync(_kafkaSettings.ListeningHistoryTopic, message, cancellationToken);
        _logger.LogInformation(
            "Listening event {EventId} published to topic {Topic} partition {Partition}",
            eventPayload.EventId,
            deliveryResult.Topic,
            deliveryResult.Partition.Value);
    }

    public void Dispose()
    {
        _producer.Flush(TimeSpan.FromSeconds(5));
        _producer.Dispose();
    }
}
