using AutoMapper;
using MusicStreamingService.BusinessLogic.Services.Albums.Models;
using MusicStreamingService.DataAccess.Postgres.Entities;

namespace MusicStreamingService.BusinessLogic.Mapper;

public class AlbumsBLProfile : Profile
{
    public AlbumsBLProfile()
    {
        CreateMap<Album, AlbumSimpleModel>()
            .ForMember(
                dest => dest.PhotoBase64,
                opt => opt.MapFrom(src => src.Photo == null ? null : Convert.ToBase64String(src.Photo)));
        CreateMap<Album, AlbumModel>()
            .ForMember(
                dest => dest.Songs, 
                opt => opt.MapFrom(src => (src.Songs ?? Enumerable.Empty<Song>()).OrderBy(s => s.TrackNumber)))
            .ForMember(
                dest => dest.PhotoBase64,
                opt => opt.MapFrom(src => src.Photo == null ? null : Convert.ToBase64String(src.Photo)));
        CreateMap<CreateAlbumModel, Album>()
            .ForMember(dest => dest.Artists, opt => opt.Ignore());
    }
}