namespace MusicStreamingService.DataAccess.Postgres.Entities;

public record PopularityCursor(
    long PlayCount,
    DateTime CreatedAt);
