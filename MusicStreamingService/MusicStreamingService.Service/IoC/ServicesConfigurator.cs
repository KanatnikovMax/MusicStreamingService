using AutoMapper;
using Microsoft.EntityFrameworkCore;
using MusicStreamingService.BusinessLogic.Services.Albums;
using MusicStreamingService.BusinessLogic.Services.Artists;
using MusicStreamingService.BusinessLogic.Services.Songs;
using MusicStreamingService.DataAccess.Context;
using MusicStreamingService.DataAccess.Repositories;
using MusicStreamingService.DataAccess.Repositories.Interfaces;
using MusicStreamingService.Service.Settings;

namespace MusicStreamingService.Service.IoC;

public static class ServicesConfigurator
{
    public static void ConfigureServices(IServiceCollection services, MusicServiceSettings settings)
    {
        // songs
        services.AddScoped<ISongsRepository>(x =>
            new SongsRepository(x.GetRequiredService<IDbContextFactory<MusicServiceDbContext>>()));
        services.AddScoped<ISongsService, SongsService>();
        // albums
        services.AddScoped<IAlbumsRepository>(x =>
            new AlbumsRepository(x.GetRequiredService<IDbContextFactory<MusicServiceDbContext>>()));
        services.AddScoped<IAlbumsService>(x =>
            new AlbumsService(x.GetRequiredService<IAlbumsRepository>(),
                x.GetRequiredService<IArtistsRepository>(),
                x.GetRequiredService<IMapper>()));
        // artists
        services.AddScoped<IArtistsRepository>(x =>
            new ArtistsRepository(x.GetRequiredService<IDbContextFactory<MusicServiceDbContext>>()));
        services.AddScoped<IArtistsService>(x =>
            new ArtistsService(x.GetRequiredService<IArtistsRepository>(),
                x.GetRequiredService<IMapper>()));
        // users
        services.AddScoped<IUsersRepository>(x =>
            new UsersRepository(x.GetRequiredService<IDbContextFactory<MusicServiceDbContext>>()));
    }
}