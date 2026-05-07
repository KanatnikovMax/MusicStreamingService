using FluentAssertions;
using Moq;
using MusicStreamingService.BusinessLogic.Exceptions;
using MusicStreamingService.BusinessLogic.Services.Albums;
using MusicStreamingService.BusinessLogic.Services.Albums.Models;
using MusicStreamingService.BusinessLogic.Services.Media.Models;
using MusicStreamingService.BusinessLogic.Tests.TestHelpers;
using MusicStreamingService.DataAccess.Postgres.Entities;

namespace MusicStreamingService.BusinessLogic.Tests.Services.Albums;

public sealed class AlbumsServiceTests
{
    private readonly BusinessLogicTestFixture _test = new();
    private readonly AlbumsService _sut;

    public AlbumsServiceTests()
    {
        _sut = new AlbumsService(
            _test.UnitOfWork.Object,
            _test.Mapper,
            _test.Cache.Object,
            _test.MediaStorage.Object);
    }

    [Fact]
    public async Task CreateAlbumAsync_WhenAlbumIsValid_ShouldSaveUploadPhotoCommitAndReturnAlbum()
    {
        var photo = new FileUploadModel
        {
            Content = new MemoryStream([1, 2, 3]),
            FileName = "cover.jpg",
            ContentType = "image/jpeg"
        };
        var model = new CreateAlbumModel
        {
            Title = "New Album",
            ReleaseDate = DateTime.UtcNow.Date,
            Artists = ["First Artist", "Second Artist"],
            Photo = photo
        };
        var artists = model.Artists
            .Select(name => new Artist { Id = Guid.NewGuid(), Name = name })
            .ToList();
        const string photoObjectKey = "albums/cover.jpg";
        const string photoUrl = "https://media/albums/cover.jpg";

        _test.Albums
            .Setup(x => x.FindByTitleAsync(model.Title))
            .ReturnsAsync((Album?)null);

        _test.Artists
            .Setup(x => x.GetOrCreateArtistsAsync(model.Artists))
            .ReturnsAsync(artists);

        _test.MediaStorage
            .Setup(x => x.UploadAsync(photo, "albums", It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(photoObjectKey);

        _test.MediaStorage
            .Setup(x => x.GetReadUrlAsync(photoObjectKey, It.IsAny<CancellationToken>()))
            .ReturnsAsync(photoUrl);

        _test.Albums
            .Setup(x => x.SaveAsync(It.IsAny<Album>()))
            .ReturnsAsync((Album album) =>
            {
                album.Id = Guid.NewGuid();
                return album;
            });

        var result = await _sut.CreateAlbumAsync(model);

        result.Title.Should().Be(model.Title);
        result.ReleaseDate.Should().Be(model.ReleaseDate);
        result.PhotoUrl.Should().Be(photoUrl);
        result.Artists.Select(x => x.Name).Should().BeEquivalentTo(model.Artists);

        _test.Albums.Verify(
            x => x.SaveAsync(It.Is<Album>(album =>
                album.Title == model.Title &&
                album.ReleaseDate == model.ReleaseDate &&
                album.PhotoObjectKey == photoObjectKey &&
                album.Artists == artists)),
            Times.Once);

        _test.MediaStorage.Verify(
            x => x.UploadAsync(photo, "albums", It.IsAny<Guid>(), It.IsAny<CancellationToken>()),
            Times.Once);

        _test.UnitOfWork.Verify(x => x.CommitAsync(), Times.Once);
        _test.UnitOfWork.Verify(x => x.RollbackAsync(), Times.Never);
    }

    [Fact]
    public async Task CreateAlbumAsync_WhenSameTitleAndSameArtistsExist_ShouldRollbackAndThrow()
    {
        var model = new CreateAlbumModel
        {
            Title = "Existing",
            ReleaseDate = DateTime.UtcNow,
            Artists = ["First Artist", "Second Artist"]
        };
        var existingAlbum = new Album
        {
            Id = Guid.NewGuid(),
            Title = model.Title,
            Artists =
            [
                new Artist { Id = Guid.NewGuid(), Name = "second artist" },
                new Artist { Id = Guid.NewGuid(), Name = "first artist" }
            ]
        };

        _test.Albums.Setup(x => x.FindByTitleAsync(model.Title)).ReturnsAsync(existingAlbum);

        var act = () => _sut.CreateAlbumAsync(model);

        await act.Should().ThrowAsync<EntityAlreadyExistsException>();
        _test.UnitOfWork.Verify(x => x.RollbackAsync(), Times.AtLeastOnce);
        _test.MediaStorage.Verify(
            x => x.UploadAsync(It.IsAny<FileUploadModel?>(), It.IsAny<string>(), It.IsAny<Guid>(),
                It.IsAny<CancellationToken>()),
            Times.Never);
        _test.Albums.Verify(x => x.SaveAsync(It.IsAny<Album>()), Times.Never);
    }

    [Fact]
    public async Task UpdateAlbumAsync_WhenAlbumDoesNotExist_ShouldRollbackAndThrow()
    {
        var albumId = Guid.NewGuid();
        var model = new UpdateAlbumModel
        {
            Title = "Updated",
            ReleaseDate = DateTime.UtcNow,
            Artists = ["Artist"]
        };

        _test.Albums.Setup(x => x.FindByIdAsync(albumId)).ReturnsAsync((Album?)null);

        var act = () => _sut.UpdateAlbumAsync(model, albumId);

        await act.Should().ThrowAsync<EntityNotFoundException>();
        _test.UnitOfWork.Verify(x => x.RollbackAsync(), Times.Once);
        _test.Albums.Verify(x => x.Update(It.IsAny<Album>()), Times.Never);
    }
}
