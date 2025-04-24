using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using MusicStreamingService.BusinessLogic.Services.Albums;
using MusicStreamingService.BusinessLogic.Services.Artists;
using MusicStreamingService.BusinessLogic.Services.Songs;
using MusicStreamingService.BusinessLogic.Services.Users;
using MusicStreamingService.DataAccess.Context;
using MusicStreamingService.DataAccess.Entities;
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
        // Unit Of Work
        services.AddScoped<IUnitOfWork, UnitOfWork>();
        // services
        services.AddScoped<IArtistsService, ArtistsService>();
        services.AddScoped<IAlbumsService, AlbumsService>();
        services.AddScoped<ISongsService, SongsService>();
        services.AddScoped<IAccountService, AccountService>();
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
    }
}