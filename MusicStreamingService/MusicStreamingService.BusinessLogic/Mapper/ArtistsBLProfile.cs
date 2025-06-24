using AutoMapper;
using MusicStreamingService.BusinessLogic.Services.Artists.Models;
using MusicStreamingService.DataAccess.Postgres.Entities;

namespace MusicStreamingService.BusinessLogic.Mapper;

public class ArtistsBLProfile : Profile
{
    public ArtistsBLProfile()
    {
        CreateMap<Artist, ArtistSimpleModel>();
        CreateMap<Artist, ArtistModel>()
            .ForMember(
                dest => dest.Albums,
                opt => opt.MapFrom(src => (src.Albums ?? Enumerable.Empty<Album>()).OrderByDescending(a => a.ReleaseDate)))
            .ForMember(
                dest => dest.PhotoBase64,
                opt => opt.MapFrom(src => src.Photo == null ? null : Convert.ToBase64String(src.Photo)));
        CreateMap<CreateArtistModel, Artist>()
            .ForMember(dest => dest.Albums, opt => opt.Ignore())
            .ForMember(dest => dest.Songs, opt => opt.Ignore());
    }
}