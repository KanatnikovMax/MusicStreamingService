using AutoMapper;
using MusicStreamingService.BusinessLogic.Services.Playlists.Models;
using MusicStreamingService.DataAccess.Postgres.Entities;

namespace MusicStreamingService.BusinessLogic.Mapper;

public class PlaylistsBLProfile : Profile
{
    public PlaylistsBLProfile()
    {
        CreateMap<Playlist, PlaylistModel>()
            .ForMember(
                dest => dest.PhotoBase64,
                opt => opt.MapFrom(src => src.Photo == null ? null : Convert.ToBase64String(src.Photo)));
        CreateMap<CreatePlaylistModel, Playlist>();
        CreateMap<UpdatePlaylistModel, Playlist>();
        CreateMap<PlaylistSong, PlaylistSongModel>();
    }
}
