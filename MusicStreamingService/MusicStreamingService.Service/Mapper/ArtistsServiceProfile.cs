using AutoMapper;
using MusicStreamingService.BusinessLogic.Services.Artists.Models;
using MusicStreamingService.Service.Controllers.Artists.Models;

namespace MusicStreamingService.Service.Mapper;

public class ArtistsServiceProfile : Profile
{
    public ArtistsServiceProfile()
    {
        CreateMap<CreateArtistRequest, CreateArtistModel>();
        CreateMap<UpdateArtistRequest, UpdateArtistModel>();
    }
}