using AutoMapper;
using MusicStreamingService.BusinessLogic.Services.Albums.Models;
using MusicStreamingService.DataAccess.Postgres.Entities;

namespace MusicStreamingService.BusinessLogic.Mapper;

public class AlbumsBLProfile : Profile
{
    public AlbumsBLProfile()
    {
        CreateMap<Album, AlbumSimpleModel>();
        CreateMap<Album, AlbumModel>()
            .ForMember(
                dest => dest.Songs, 
                opt => opt.MapFrom(src => (src.Songs ?? Enumerable.Empty<Song>()).OrderBy(s => s.TrackNumber)));
        CreateMap<CreateAlbumModel, Album>()
            .ForMember(dest => dest.Artists, opt => opt.Ignore());
    }
}