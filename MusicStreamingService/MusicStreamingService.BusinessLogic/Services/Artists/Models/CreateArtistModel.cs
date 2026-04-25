using MusicStreamingService.BusinessLogic.Services.Media.Models;

namespace MusicStreamingService.BusinessLogic.Services.Artists.Models;

public class CreateArtistModel
{
    public string Name { get; set; }
    public FileUploadModel? Photo { get; set; }
}
