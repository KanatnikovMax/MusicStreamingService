using Cassandra;
using MusicStreamingService.DataAccess.Cassandra.Repositories.Interfaces;

namespace MusicStreamingService.DataAccess.Cassandra.Repositories;

public class CassandraSongsRepository : ICassandraSongsRepository
{
    private readonly ISession _session;

    public CassandraSongsRepository(ISession session)
    {
        _session = session;
    }

    public async Task SaveAsync(Guid songId, byte[] data)
    {
        const string query = """
                             INSERT INTO songs (song_id, data)
                             VALUES (?, ?)
                             """;
        
        var preparedStatement = await _session.PrepareAsync(query);
        var statement = preparedStatement.Bind(songId, data);
        
        await _session.ExecuteAsync(statement);
    }

    public async Task DeleteAsync(Guid songId)
    {
        const string query = """
                             DELETE FROM songs
                              WHERE song_id = ?
                             """;

        var preparedStatement = await _session.PrepareAsync(query);
        var statement = preparedStatement.Bind(songId);
        
        await _session.ExecuteAsync(statement);
    }

    public async Task<byte[]?> FindAsync(Guid songId)
    {
        const string query = """
                             SELECT data
                               FROM songs
                              WHERE song_id = ?
                             """;
        
        var preparedStatement = await _session.PrepareAsync(query);
        var statement = preparedStatement.Bind(songId);
        
        var result = await _session.ExecuteAsync(statement);
        var row = result.FirstOrDefault();
        
        return row?["data"] as byte[];
    }
}