using AutoMapper;
using MusicStreamingService.BusinessLogic.Services.Artists.Models;
using MusicStreamingService.DataAccess.Entities;

namespace MusicStreamingService.BusinessLogic.Mapper;

public class ArtistsBLProfile : Profile
{
    public ArtistsBLProfile()
    {
        CreateMap<Artist, ArtistSimpleModel>();
        CreateMap<Artist, ArtistModel>()
            .ForMember(
                dest => dest.Albums,
                opt => opt.MapFrom(src => (src.Albums ?? Enumerable.Empty<Album>()).OrderByDescending(a => a.ReleaseDate)));
        CreateMap<CreateArtistModel, Artist>()
            .ForMember(dest => dest.Albums, opt => opt.Ignore())
            .ForMember(dest => dest.Songs, opt => opt.Ignore());
        CreateMap<UpdateArtistModel, Artist>()
            .ForMember(dest => dest.Albums, opt => opt.Ignore())
            .ForMember(dest => dest.Songs, opt => opt.Ignore());
    }
}