using AutoMapper;
using MusicStreamingService.BusinessLogic.Services.Playlists.Models;
using MusicStreamingService.DataAccess.Postgres.Entities;

namespace MusicStreamingService.BusinessLogic.Mapper;

public class PlaylistsBLProfile : Profile
{
    public PlaylistsBLProfile()
    {
        CreateMap<Playlist, PlaylistModel>();
        CreateMap<CreatePlaylistModel, Playlist>();
        CreateMap<UpdatePlaylistModel, Playlist>();
        CreateMap<PlaylistSong, PlaylistSongModel>();
    }
}
