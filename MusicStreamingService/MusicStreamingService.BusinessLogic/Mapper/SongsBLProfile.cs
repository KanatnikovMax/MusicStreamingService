using AutoMapper;
using MusicStreamingService.BusinessLogic.Services.Songs.Models;
using MusicStreamingService.DataAccess.Entities;

namespace MusicStreamingService.BusinessLogic.Mapper;

public class SongsBLProfile : Profile
{
    public SongsBLProfile()
    {
        CreateMap<Song, SongSimpleModel>();
        CreateMap<Song, SongModel>();
        CreateMap<CreateSongModel, Song>()
            .ForMember(dest => dest.Artists, opt => opt.Ignore())
            .ForMember(dest => dest.Album, opt => opt.Ignore());
        CreateMap<UpdateSongModel, Song>()
            .ForMember(dest => dest.Artists, opt => opt.Ignore())
            .ForMember(dest => dest.Album, opt => opt.Ignore());
    }
}