namespace MusicStreamingService.DataAccess.Cassandra.Repositories.Interfaces;

public interface ICassandraSongsRepository
{
    Task SaveAsync(Guid songId, byte[] data);
    Task DeleteAsync(Guid songId);
    Task<byte[]?> FindAsync(Guid songId);
}