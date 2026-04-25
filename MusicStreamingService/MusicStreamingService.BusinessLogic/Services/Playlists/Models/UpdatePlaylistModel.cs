using MusicStreamingService.BusinessLogic.Services.Media.Models;

namespace MusicStreamingService.BusinessLogic.Services.Playlists.Models;

public class UpdatePlaylistModel
{
    public string? Name { get; set; }
    public FileUploadModel? Photo { get; set; }
}
