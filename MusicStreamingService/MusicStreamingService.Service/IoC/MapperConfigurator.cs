using MusicStreamingService.BusinessLogic.Mapper;
using MusicStreamingService.Service.Mapper;

namespace MusicStreamingService.Service.IoC;

public static class MapperConfigurator
{
    public static void ConfigureServices(IServiceCollection services)
    {
        services.AddAutoMapper(config =>
        {
            // artists
            config.AddProfile<ArtistsBLProfile>();
            config.AddProfile<ArtistsServiceProfile>();
            // albums
            config.AddProfile<AlbumsBLProfile>();
            config.AddProfile<AlbumsServiceProfile>();
            // songs
            config.AddProfile<SongsBLProfile>();
            config.AddProfile<SongsServiceProfile>();
            // users
            config.AddProfile<UsersBLProfile>();
            config.AddProfile<UsersServiceProfile>();
            // pagination
            config.AddProfile<PaginationServiceProfile>();
        });
    }
}