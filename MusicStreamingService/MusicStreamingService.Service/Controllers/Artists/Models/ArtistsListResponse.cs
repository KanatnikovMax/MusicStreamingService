using MusicStreamingService.DataAccess.Entities;

namespace MusicStreamingService.Service.Controllers.Artists.Models;

public record ArtistsListResponse(List<Artist> Artists);