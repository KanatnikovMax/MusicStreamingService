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
        services.AddScoped<ISongsRepository, SongsRepository>();
        services.AddScoped<ISongsService, SongsService>();
        // albums
        services.AddScoped<IAlbumsRepository, AlbumsRepository>();
        services.AddScoped<IAlbumsService, AlbumsService>();
        // artists
        services.AddScoped<IArtistsRepository, ArtistsRepository>();
        services.AddScoped<IArtistsService, ArtistsService>();
        // users
        services.AddScoped<IUsersRepository, UsersRepository>();
    }
}