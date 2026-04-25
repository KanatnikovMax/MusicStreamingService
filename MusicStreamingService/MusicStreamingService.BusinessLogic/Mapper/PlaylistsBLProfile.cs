using AutoMapper;
using MusicStreamingService.BusinessLogic.Services.Playlists.Models;
using MusicStreamingService.DataAccess.Postgres.Entities;

namespace MusicStreamingService.BusinessLogic.Mapper;

public class PlaylistsBLProfile : Profile
{
    public PlaylistsBLProfile()
    {
        CreateMap<Playlist, PlaylistModel>();
        CreateMap<CreatePlaylistModel, Playlist>()
            .ForMember(dest => dest.PhotoObjectKey, opt => opt.Ignore());
        CreateMap<UpdatePlaylistModel, Playlist>()
            .ForMember(dest => dest.PhotoObjectKey, opt => opt.Ignore());
        CreateMap<PlaylistSong, PlaylistSongModel>();
    }
}
