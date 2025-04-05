using AutoMapper;
using MusicStreamingService.BusinessLogic.Services.Albums.Models;
using MusicStreamingService.Service.Controllers.Albums.Models;

namespace MusicStreamingService.Service.Mapper;

public class AlbumsServiceProfile : Profile
{
    public AlbumsServiceProfile()
    {
        CreateMap<CreateAlbumRequest, CreateAlbumModel>();
        CreateMap<UpdateAlbumRequest, CreateAlbumModel>();
    }
}