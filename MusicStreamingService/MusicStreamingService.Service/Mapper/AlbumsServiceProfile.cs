using AutoMapper;
using MusicStreamingService.BusinessLogic.Services.Albums.Models;
using MusicStreamingService.Service.Controllers.Requests.Albums;

namespace MusicStreamingService.Service.Mapper;

public class AlbumsServiceProfile : Profile
{
    public AlbumsServiceProfile()
    {
        CreateMap<CreateAlbumRequest, CreateAlbumModel>()
            .ForMember(dest => dest.Photo, opt => opt.Ignore());
        
        CreateMap<UpdateAlbumRequest, UpdateAlbumModel>()
            .ForMember(dest => dest.Photo, opt => opt.Ignore());
    }
}