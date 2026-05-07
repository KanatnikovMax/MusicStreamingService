using FluentAssertions;
using Microsoft.AspNetCore.Http;
using MusicStreamingService.Service.Utils;

namespace MusicStreamingService.Service.Tests.Utils;

public sealed class PhotoFilesUtilTests
{
    [Fact]
    public async Task CreateFileUploadModelAsync_ShouldCopyFormFileMetadataAndContent()
    {
        var content = new byte[] { 1, 2, 3, 4 };
        var formFile = new FormFile(new MemoryStream(content), 0, content.Length, "photo", "cover.jpg")
        {
            Headers = new HeaderDictionary(),
            ContentType = "image/jpeg"
        };

        var result = await PhotoFilesUtil.CreateFileUploadModelAsync(formFile, CancellationToken.None);

        result.FileName.Should().Be("cover.jpg");
        result.ContentType.Should().Be("image/jpeg");
        result.Content.Position.Should().Be(0);

        using var memoryStream = new MemoryStream();
        await result.Content.CopyToAsync(memoryStream);
        memoryStream.ToArray().Should().Equal(content);
    }
}
