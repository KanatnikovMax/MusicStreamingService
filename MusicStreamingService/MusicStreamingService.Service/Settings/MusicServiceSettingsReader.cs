namespace MusicStreamingService.Service.Settings;

public static class MusicServiceSettingsReader
{
    public static MusicServiceSettings ReadSettings(IConfiguration configuration)
    {
        return new MusicServiceSettings
        {
            MusicServiceDbConnectionString = configuration.GetValue<string>("MusicServiceDbContext"),
            IdentityServerUri = configuration.GetValue<string>("IdentityServerSettings:Uri"),
            ClientId = configuration.GetValue<string>("IdentityServerSettings:ClientId"),
            ClientSecret = configuration.GetValue<string>("IdentityServerSettings:ClientSecret"),
            MasterAdminEmail = configuration.GetValue<string>("IdentityServerSettings:MasterAdminEmail"),
            MasterAdminPassword = configuration.GetValue<string>("IdentityServerSettings:MasterAdminPassword"),
            CassandraContactPoints = configuration.GetSection("Cassandra:ContactPoints").Get<string[]>(),
            CassandraKeyspace = configuration.GetValue<string>("Cassandra:Keyspace"),
            CassandraPort = int.Parse(configuration.GetValue<string>("Cassandra:Port")),
            CassandraReplicationFactor = int.Parse(configuration.GetValue<string>("Cassandra:ReplicationFactor")),
            FrontendUrl = configuration.GetValue<string>("Cors:FrontendUrl"),
        };
    }
}