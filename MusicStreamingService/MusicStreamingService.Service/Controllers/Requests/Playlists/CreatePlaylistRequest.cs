using System.ComponentModel.DataAnnotations;

namespace MusicStreamingService.Service.Controllers.Requests.Playlists;

public class CreatePlaylistRequest
{
    [Required]
    [MaxLength(200)]
    public string Name { get; set; }
}
