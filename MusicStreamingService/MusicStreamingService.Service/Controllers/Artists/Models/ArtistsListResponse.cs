using MusicStreamingService.BusinessLogic.Services.Artists.Models;

namespace MusicStreamingService.Service.Controllers.Artists.Models;

public record ArtistsListResponse(List<ArtistModel> Artists);