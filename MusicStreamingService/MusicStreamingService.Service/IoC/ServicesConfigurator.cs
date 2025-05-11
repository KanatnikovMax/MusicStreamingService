using AutoMapper;
using Microsoft.AspNetCore.Identity;
using MusicStreamingService.BusinessLogic.Services.Albums;
using MusicStreamingService.BusinessLogic.Services.Artists;
using MusicStreamingService.BusinessLogic.Services.Songs;
using MusicStreamingService.BusinessLogic.Services.Users;
using MusicStreamingService.DataAccess.Cassandra.Repositories;
using MusicStreamingService.DataAccess.Cassandra.Repositories.Interfaces;
using MusicStreamingService.DataAccess.Postgres.Context;
using MusicStreamingService.DataAccess.Postgres.Entities;
using MusicStreamingService.DataAccess.Postgres.UnitOfWork;
using MusicStreamingService.DataAccess.Postgres.UnitOfWork.Interfaces;
using MusicStreamingService.Service.Init;
using MusicStreamingService.Service.Settings;

namespace MusicStreamingService.Service.IoC;

public static class ServicesConfigurator
{
    public static void ConfigureServices(IServiceCollection services, MusicServiceSettings settings)
    {
        // Unit Of Work
        services.AddScoped<IUnitOfWork, UnitOfWork>();
        // services
        services.AddScoped<IArtistsService, ArtistsService>();
        services.AddScoped<IAlbumsService, AlbumsService>();
        services.AddScoped<ISongsService, SongsService>();
        services.AddScoped<IAccountService, AccountService>();
        services.AddScoped<IUsersActionsService, UsersActionsService>();
        services.AddScoped<IAuthService>(x =>
            new AuthService(
                x.GetRequiredService<MusicServiceDbContext>(),
                x.GetRequiredService<SignInManager<User>>(),
                x.GetRequiredService<UserManager<User>>(),
                x.GetRequiredService<IHttpClientFactory>(),
                x.GetRequiredService<IMapper>(),
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
        // cassandra
        services.AddScoped<ICassandraSongsRepository>(x =>
            new CassandraSongsRepository(x.GetRequiredService<CassandraCluster>().Session));
    }
}