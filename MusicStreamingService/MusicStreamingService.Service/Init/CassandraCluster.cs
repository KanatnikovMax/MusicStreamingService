using Cassandra;
using ISession = Cassandra.ISession;

namespace MusicStreamingService.Service.Init;

public class CassandraCluster : IDisposable
{
    private readonly ILogger<CassandraCluster> _logger;
    private readonly Cluster _cluster;
    private readonly string _keyspace;
    private readonly int _replicationFactor;
    public ISession Session { get; private set; }

    public CassandraCluster(ILogger<CassandraCluster> logger, string[] contactPoints, string keyspace, int port,
        int replicationFactor)
    {
        _logger = logger;
        _keyspace = keyspace;
        _replicationFactor = replicationFactor;
        
        _cluster = Cluster.Builder()
            .AddContactPoints(contactPoints)
            .WithPort(port)
            .Build();
    }

    public async Task InitializeAsync()
    {
        try
        {
            await CreateKeyspaceAsync();
            
            Session = await _cluster.ConnectAsync(_keyspace);

            await CreateTableAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Cassandra initialization failed");
            throw;
        }
    }

    private async Task CreateKeyspaceAsync()
    {
        var query = $$"""
                      CREATE KEYSPACE IF NOT EXISTS {{_keyspace}} 
                      WITH REPLICATION = {
                          'class': 'SimpleStrategy',
                          'replication_factor': {{_replicationFactor}}
                      }
                      """;

        using (var systemSession = await _cluster.ConnectAsync())
        {
            await systemSession.ExecuteAsync(new SimpleStatement(query));
        }
        
        _logger.LogInformation("Keyspace created/validated");
    }

    private async Task CreateTableAsync()
    {
        await Session.ExecuteAsync(new SimpleStatement($"USE {_keyspace}"));

        const string query = """
                             CREATE TABLE IF NOT EXISTS songs (
                                 song_id uuid PRIMARY KEY,
                                 data blob
                             )
                             """;

        await Session.ExecuteAsync(new SimpleStatement(query));
        
        _logger.LogInformation("Table 'songs' created/validated");
    }
    
    public void Dispose()
    {
        _cluster?.Dispose();
    }
}