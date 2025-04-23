using AutoMapper;
using MusicStreamingService.BusinessLogic.Services.Artists.Models;
using MusicStreamingService.Service.Controllers.Requests.Artists;

namespace MusicStreamingService.Service.Mapper;

public class ArtistsServiceProfile : Profile
{
    public ArtistsServiceProfile()
    {
        CreateMap<CreateArtistRequest, CreateArtistModel>();
        CreateMap<UpdateArtistRequest, UpdateArtistModel>();
    }
}