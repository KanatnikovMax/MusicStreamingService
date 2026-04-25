using MusicStreamingService.BusinessLogic.Services.Media.Models;

namespace MusicStreamingService.MediaLibrary;

public interface IMediaStorageService
{
    Task<string?> UploadAsync(FileUploadModel? file, string entityType, Guid entityId, CancellationToken cancellationToken = default);
    Task<string?> GetReadUrlAsync(string? objectKey, CancellationToken cancellationToken = default);
    Task DeleteAsync(string? objectKey, CancellationToken cancellationToken = default);
}
