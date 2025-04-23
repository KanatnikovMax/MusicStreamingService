using MusicStreamingService.BusinessLogic.Services.Artists.Models;

namespace MusicStreamingService.Service.Controllers.Responses.Artists;

public record ArtistsListResponse(List<ArtistModel> Artists);