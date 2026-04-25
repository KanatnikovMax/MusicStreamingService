using MusicStreamingService.BusinessLogic.Services.Media.Models;

namespace MusicStreamingService.Service.Utils;

public static class PhotoFilesUtil
{
    public static async Task<FileUploadModel> CreateFileUploadModelAsync(IFormFile photo, 
        CancellationToken cancellationToken)
    {
        var memoryStream = new MemoryStream();
        await photo.CopyToAsync(memoryStream, cancellationToken);
        memoryStream.Position = 0;

        return new FileUploadModel
        {
            Content = memoryStream,
            ContentType = photo.ContentType,
            FileName = photo.FileName
        };
    }
}