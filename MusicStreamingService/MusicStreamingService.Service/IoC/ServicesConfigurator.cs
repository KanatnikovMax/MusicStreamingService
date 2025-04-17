using AutoMapper;
using Microsoft.EntityFrameworkCore;
using MusicStreamingService.BusinessLogic.Services.Albums;
using MusicStreamingService.BusinessLogic.Services.Artists;
using MusicStreamingService.BusinessLogic.Services.Songs;
using MusicStreamingService.DataAccess.Context;
using MusicStreamingService.DataAccess.Repositories;
using MusicStreamingService.DataAccess.Repositories.Interfaces;
using MusicStreamingService.DataAccess.UnitOfWork;
using MusicStreamingService.DataAccess.UnitOfWork.Interfaces;
using MusicStreamingService.Service.Settings;

namespace MusicStreamingService.Service.IoC;

public static class ServicesConfigurator
{
    public static void ConfigureServices(IServiceCollection services, MusicServiceSettings settings)
    {
        // repositories
        /*services.AddScoped<ISongsRepository, SongsRepository>();
        services.AddScoped<IAlbumsRepository, AlbumsRepository>();
        services.AddScoped<IArtistsRepository, ArtistsRepository>();
        services.AddScoped<IUsersRepository, UsersRepository>();*/
        // Unit Of Work
        services.AddScoped<IUnitOfWork, UnitOfWork>();
        // services
        services.AddScoped<IArtistsService, ArtistsService>();
        services.AddScoped<IAlbumsService, AlbumsService>();
        services.AddScoped<ISongsService, SongsService>();
    }
}