using AutoMapper;
using MusicStreamingService.BusinessLogic.Services.Songs.Models;
using MusicStreamingService.DataAccess.Postgres.Entities;

namespace MusicStreamingService.BusinessLogic.Mapper;

public class SongsBLProfile : Profile
{
    public SongsBLProfile()
    {
        CreateMap<Song, SongSimpleModel>();
        CreateMap<Song, SongModel>();
        CreateMap<CreateSongModel, Song>()
            .ForMember(dest => dest.Artists, opt => opt.Ignore());
    }
}