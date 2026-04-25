namespace MusicStreamingService.BusinessLogic.Services.Media.Models;

public sealed class FileUploadModel
{
    public required Stream Content { get; init; }
    public required string FileName { get; init; }
    public required string ContentType { get; init; }
}
