using AutoMapper;
using MusicStreamingService.BusinessLogic.Services.Playlists.Models;
using MusicStreamingService.Service.Controllers.Requests.Playlists;

namespace MusicStreamingService.Service.Mapper;

public class PlaylistsServiceProfile : Profile
{
    public PlaylistsServiceProfile()
    {
        CreateMap<CreatePlaylistRequest, CreatePlaylistModel>()
            .ForMember(dest => dest.Photo, opt => opt.Ignore());
        CreateMap<UpdatePlaylistRequest, UpdatePlaylistModel>()
            .ForMember(dest => dest.Photo, opt => opt.Ignore());
    }
}
