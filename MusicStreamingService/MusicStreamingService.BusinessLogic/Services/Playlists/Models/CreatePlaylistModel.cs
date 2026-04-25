using MusicStreamingService.BusinessLogic.Services.Media.Models;

namespace MusicStreamingService.BusinessLogic.Services.Playlists.Models;

public class CreatePlaylistModel
{
    public string Name { get; set; }
    public FileUploadModel? Photo { get; set; }
}
