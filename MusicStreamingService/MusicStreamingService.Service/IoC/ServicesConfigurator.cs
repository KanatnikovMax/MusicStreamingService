using Microsoft.EntityFrameworkCore;
using MusicStreamingService.DataAccess.Context;
using MusicStreamingService.DataAccess.Repositories;
using MusicStreamingService.DataAccess.Repositories.Interfaces;
using MusicStreamingService.Service.Settings;

namespace MusicStreamingService.Service.IoC;

public static class ServicesConfigurator
{
    public static void ConfigureServices(IServiceCollection services, MusicServiceSettings settings)
    {
        services.AddScoped<ISongsRepository>(x =>
            new SongsRepository(x.GetRequiredService<IDbContextFactory<MusicServiceDbContext>>()));
        services.AddScoped<IAlbumsRepository>(x =>
            new AlbumsRepository(x.GetRequiredService<IDbContextFactory<MusicServiceDbContext>>()));
        services.AddScoped<IArtistsRepository>(x =>
            new ArtistsRepository(x.GetRequiredService<IDbContextFactory<MusicServiceDbContext>>()));
        services.AddScoped<IUsersRepository>(x =>
            new UsersRepository(x.GetRequiredService<IDbContextFactory<MusicServiceDbContext>>()));
    }
}