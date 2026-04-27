using AutoMapper;
using Minio;
using Microsoft.AspNetCore.Identity;
using MusicStreamingService.BusinessLogic.Services.Albums;
using MusicStreamingService.BusinessLogic.Services.Artists;
using MusicStreamingService.BusinessLogic.Services.Playlists;
using MusicStreamingService.BusinessLogic.Services.Songs;
using MusicStreamingService.BusinessLogic.Services.Users;
using MusicStreamingService.DataAccess.Cassandra.Repositories;
using MusicStreamingService.DataAccess.Cassandra.Repositories.Interfaces;
using MusicStreamingService.DataAccess.Postgres.Context;
using MusicStreamingService.DataAccess.Postgres.Entities;
using MusicStreamingService.DataAccess.Postgres.UnitOfWork;
using MusicStreamingService.DataAccess.Postgres.UnitOfWork.Interfaces;
using MusicStreamingService.Infrastructure.Kafka.ListeningHistory;
using MusicStreamingService.MediaLibrary;
using MusicStreamingService.Service.Init;
using MusicStreamingService.Service.Settings;

namespace MusicStreamingService.Service.IoC;

public static class ServicesConfigurator
{
    public static void ConfigureServices(IServiceCollection services, MusicServiceSettings settings)
    {
        services.AddSingleton(settings.MinioSettings);
        services.AddSingleton(settings.KafkaSettings);
        services.AddSingleton(settings);
        services.AddSingleton<IMinioClient>(_ =>
        {
            var clientBuilder = new MinioClient()
                .WithEndpoint(settings.MinioSettings.Endpoint)
                .WithCredentials(settings.MinioSettings.AccessKey, settings.MinioSettings.SecretKey);

            if (settings.MinioSettings.UseSsl)
            {
                clientBuilder = clientBuilder.WithSSL();
            }

            return clientBuilder.Build();
        });
        services.AddSingleton<IMediaStorageService, MinioMediaStorageService>();

        services.AddScoped<IUnitOfWork, UnitOfWork>();
        services.AddScoped<IArtistsService, ArtistsService>();
        services.AddScoped<IAlbumsService, AlbumsService>();
        services.AddScoped<ISongsService, SongsService>();
        services.AddScoped<IPlaylistsService, PlaylistsService>();
        services.AddScoped<IAccountService, AccountService>();
        services.AddScoped<IUsersActionsService, UsersActionsService>();
        services.AddSingleton<IListeningHistoryProducer, KafkaListeningHistoryProducer>();
        services.AddScoped<IAuthService>(x =>
            new AuthService(
                x.GetRequiredService<MusicServiceDbContext>(),
                x.GetRequiredService<SignInManager<User>>(),
                x.GetRequiredService<UserManager<User>>(),
                x.GetRequiredService<IHttpClientFactory>(),
                x.GetRequiredService<IMapper>(),
                x.GetRequiredService<ILogger<AuthService>>(),
                settings.IdentityServerUri,
                settings.ClientId,
                settings.ClientSecret));
        services.AddSingleton<CassandraCluster>(x =>
            new CassandraCluster(
                x.GetRequiredService<ILogger<CassandraCluster>>(),
                settings.CassandraContactPoints,
                settings.CassandraKeyspace,
                settings.CassandraPort,
                settings.CassandraReplicationFactor));
        services.AddScoped<ICassandraSongsRepository>(x =>
            new CassandraSongsRepository(x.GetRequiredService<CassandraCluster>().Session));
    }
}
