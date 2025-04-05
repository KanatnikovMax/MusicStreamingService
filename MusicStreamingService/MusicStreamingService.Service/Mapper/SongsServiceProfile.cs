using AutoMapper;
using MusicStreamingService.BusinessLogic.Services.Songs.Models;
using MusicStreamingService.Service.Controllers.Songs.Models;

namespace MusicStreamingService.Service.Mapper;

public class SongsServiceProfile : Profile
{
    public SongsServiceProfile()
    {
        CreateMap<CreateSongRequest, CreateSongModel>();
        CreateMap<UpdateSongRequest, UpdateSongModel>();
    }
}