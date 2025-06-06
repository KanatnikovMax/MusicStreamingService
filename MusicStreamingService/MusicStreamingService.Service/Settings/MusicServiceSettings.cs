﻿namespace MusicStreamingService.Service.Settings;

public class MusicServiceSettings
{
    public string MusicServiceDbConnectionString { get; set; }
    public string IdentityServerUri { get; set; }
    public string ClientId { get; set; }
    public string ClientSecret { get; set; }
    public string MasterAdminEmail { get; set; }
    public string MasterAdminPassword { get; set; }
    public string[] CassandraContactPoints { get; set; }
    public string CassandraKeyspace { get; set; }
    public int CassandraPort { get; set; }
    public int CassandraReplicationFactor { get; set; }
    public string FrontendUrl { get; set; }
    public string RedisConnectionString { get; set; }
    public string RedisInstanceName { get; set; }
}